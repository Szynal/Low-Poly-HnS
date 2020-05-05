namespace LowPolyHnS.Variables
{
    public static class GlobalVariablesUtilities
    {
        public static Variable Get(string name)
        {
            return GlobalVariablesManager.Instance.Get(name);
        }
    }
}