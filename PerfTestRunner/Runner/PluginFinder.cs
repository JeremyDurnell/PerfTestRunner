using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PerfTestRunner.Common;
using PerfTestRunner.Common.Runner;

namespace PerfTestRunner.Runner
{
    /// <summary>
    /// Safely identifies assemblies within a designated plugin directory that contain qualifying plugin types.
    /// Based on Tim Coulter's work: http://stackoverflow.com/questions/4145713/looking-for-a-practical-approach-to-sandboxing-net-plugins
    /// </summary>
    internal class PluginFinder : MarshalByRefObject
    {
        private readonly Type _pluginBaseType;
        private readonly string _pluginPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginFinder"/> class.
        /// </summary>
        public PluginFinder(string pluginPath)
        {
            _pluginPath = pluginPath;

            // For some reason, compile-time types are not reference equal to the corresponding types referenced
            // in each plugin assembly, so equality must be tested by loading types by name from the Interop assembly.
            var interopAssemblyFile = Path.GetFullPath(Path.Combine(_pluginPath, typeof(PerfTest).Assembly.GetName().Name) + ".dll");
            var interopAssembly = Assembly.LoadFrom(interopAssemblyFile);
            _pluginBaseType = interopAssembly.GetType(typeof(PerfTest).FullName);
        }

        /// <summary>
        /// Returns the name and assembly name of qualifying plugin classes found in assemblies within the designated plugin directory.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TypeLocator}"/> that represents the qualifying plugin types.</returns>
        public static IEnumerable<TypeLocator> FindPlugins(string pluginPath)
        {
            AppDomain domain = null;

            try
            {
                domain = AppDomain.CreateDomain("Discovery Domain");

                var finder1 = (PluginFinder) domain.CreateInstanceAndUnwrap(typeof (PluginFinder).Assembly.FullName
                                                                            , typeof (PluginFinder).FullName
                                                                            , false
                                                                            , 0
                                                                            , null
                                                                            , new object[] {pluginPath}
                                                                            , null
                                                                            , null);

                return finder1.Find();
            }
            finally
            {
                if (domain != null)
                {
                    AppDomain.Unload(domain);
                }
            }
        }

        /// <summary>
        /// Surveys the configured plugin path and returns the the set of types that qualify as plugin classes.
        /// </summary>
        /// <remarks>
        /// Since this method loads assemblies, it must be called from within a dedicated application domain that is subsequently unloaded.
        /// </remarks>
        private IEnumerable<TypeLocator> Find()
        {
            var result = new List<TypeLocator>();

            foreach (var file in Directory.GetFiles(Path.GetFullPath(_pluginPath), "*.dll"))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);

                    foreach (var type in assembly
                        .GetExportedTypes()
                        .Where(type => !type.Equals(_pluginBaseType) 
                            && _pluginBaseType.IsAssignableFrom(type) 
                            && !type.IsAbstract))
                    {
                        var attributes = type.GetCustomAttributes(true).Where(a =>
                        {
                            Type attribType = a.GetType();
                            return attribType.BaseType != null && attribType.BaseType.FullName == typeof(PerfTestAttribute).FullName;
                        }).Cast<Attribute>().ToArray();

                        result.Add( new TypeLocator(assembly.FullName, type.FullName, attributes) );
                    }
                }
                catch (Exception e)
                {
                    // Ignore DLLs that are not .NET assemblies.
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Encapsulates the assembly name and type name for a <see cref="Type"/> in a serializable format.
    /// </summary>
    [Serializable]
    internal class TypeLocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeLocator"/> class.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly containing the target type.</param>
        /// <param name="typeName">The name of the target type.</param>
        /// <param name="attributes">Custom attributes present on type.</param>
        public TypeLocator(
            string assemblyName,
            string typeName, 
            Attribute[] attributes)
        {
            if (string.IsNullOrEmpty(assemblyName)) throw new ArgumentNullException("assemblyName");
            if (string.IsNullOrEmpty(typeName)) throw new ArgumentNullException("typeName");

            AssemblyName = assemblyName;
            TypeName = typeName;
            Attributes = attributes;
        }

        /// <summary>
        /// Gets the name of the assembly containing the target type.
        /// </summary>
        public string AssemblyName { get; private set; }

        /// <summary>
        /// Gets the name of the target type.
        /// </summary>
        public string TypeName { get; private set; }

        public Attribute[] Attributes { get; set; }
    }
}
