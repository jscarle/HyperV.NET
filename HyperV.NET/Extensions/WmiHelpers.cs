using System.Management;

namespace HyperV.Extensions
{
    internal static class WmiHelpers
    {
        internal static void Dispose(this ManagementObject[] array)
        {
            foreach (ManagementObject managementObject in array)
                managementObject.Dispose();
        }

        internal static ManagementObject First(this ManagementObjectCollection collection)
        {
            foreach (ManagementObject managementObject in collection)
                return managementObject;

            return null;
        }

        internal static ManagementObject[] ToObjectArray(this string[] managementStrings)
        {
            ManagementObject[] managementObjects = new ManagementObject[managementStrings.Length];
            for (int index = 0; index < managementStrings.Length; index++)
                managementObjects[index] = new ManagementObject(managementStrings[index]);
            return managementObjects;
        }

        internal static string[] ToStringArray(this ManagementObject[] managementObjects)
        {
            string[] managementStrings = new string[managementObjects.Length];
            for (int index = 0; index < managementObjects.Length; index++)
                managementStrings[index] = managementObjects[index].GetText(TextFormat.WmiDtd20);
            return managementStrings;
        }
    }
}