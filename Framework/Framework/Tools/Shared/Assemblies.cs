namespace Framework;

public class Assemblies
{
    public static List<Assembly> Get(params List<string> assemblies)
    {
        List<Assembly> result = [];
        DependencyContext
            .Default?
            .RuntimeLibraries?
            .ToList()
            .ForEach(item =>
            {
                if (IsCandidateCompilationLibrary(item, assemblies))
                {
                    var assembly = Assembly.Load(new AssemblyName(item.Name));
                    result.Add(assembly);
                }
            });
        return result;
    }

    private static bool IsCandidateCompilationLibrary(RuntimeLibrary compilationLibrary, List<string> assmblyName)
        => assmblyName
            .Any(d => compilationLibrary.Name.Contains(d)) ||
        compilationLibrary.Dependencies.Any(d => assmblyName.Any(c => d.Name.Contains(c)));
}
