namespace Backend.CodeExecution
{
    public static class CodeHelper
    {
        public static string GetCodeWithDLL(string body)
        {
            return $@"
using SystemDll;
{body}";
        }
    }
}
