using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UEPE
{
    public class UEPE : IPuckMod
    {
        public static List<Assembly> depAssemblies;
        private static string depsDirPath;
        private static string depPath;


        public bool OnEnable()
        {
            Debug.Log("Loading dependencies...");
            try
            {
                depAssemblies = new List<Assembly>();
                depsDirPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "deps");
                if (!Path.IsPathFullyQualified(depsDirPath))
                {
                    Debug.LogError($"Dependency folder {depsDirPath} is not fully qualified!");
                    return false;
                }
                Debug.Log($"Loading dependencies from {depsDirPath}");

                foreach (string dep in Constants.modDeps)
                {
                    depPath = Path.Combine(depsDirPath, dep);
                    if (File.Exists(depPath))
                    {
                        depAssemblies.Add(Assembly.LoadFile(depPath));
                    }
                    else
                    {
                        Debug.LogError($"Assembly {dep} not found at {depPath}!");
                        return false;
                    }
                }

            }
            catch (Exception e)
            {
                Debug.LogError($"UEPE failed to load deps: {e}");
                return false;
            }

            Debug.Log("Assemblies loaded:");
            int i = 0;
            foreach (Assembly asmb in depAssemblies)
            {
                Debug.Log($"{asmb.GetName()} with index {i}");
                i++;
            }

            Debug.Log("Patching and instantiating UnityExplorer...");
            try
            {
                Type expType = depAssemblies[1].GetType("UnityExplorer.ExplorerStandalone");
                MethodInfo explorerMethod = expType.GetMethod("CreateInstance", new Type[] { });
                object explorer = explorerMethod.Invoke(new object(), new object[] { });
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"UEPE failed to enable: {e}");
                return false;
            }
        }

        public bool OnDisable()
        {
            try
            {
                // unityExplorer = null;
                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"UEPE failed to disable: {e}");
                return false;
            }
        }

        void LogListener(string logMessage, LogType logType)
        {
            Debug.Log($"UEPE {logType}: {logMessage}");
        }

    }
}
