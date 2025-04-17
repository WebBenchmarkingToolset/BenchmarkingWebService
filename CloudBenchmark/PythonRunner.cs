using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting;

namespace CloudBenchmark
{
    public class PythonRunner
    {
        public ScriptEngine _engine=Python.CreateEngine();

        public TResult RunFromString<TResult>(string code, string variableName)
        {
            // for easier debugging write it out to a file and call: _engine.CreateScriptSourceFromFile(filePath);
            ScriptSource source = _engine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
            CompiledCode cc = source.Compile();

            ScriptScope scope = _engine.CreateScope();
            cc.Execute(scope);

            return scope.GetVariable<TResult>(variableName);
        }
    }
}
