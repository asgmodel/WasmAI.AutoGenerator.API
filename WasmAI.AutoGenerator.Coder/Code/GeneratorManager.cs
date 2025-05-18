namespace AutoGenerator.Code;

public class GeneratorManager
{
    private Dictionary<string, ITGenerator> generators = new Dictionary<string, ITGenerator>();

    public void RegisterGenerator(string generatorName, ITGenerator generator)
    {
        generators[generatorName] = generator;
    }

    public ITGenerator GetGenerator(string generatorName)
    {
        if (generators.TryGetValue(generatorName, out ITGenerator generator))
        {
            return generator;
        }
        else
        {
            return null; // أو يمكنك طرح استثناء
        }
    }

    public string GenerateCode(string generatorName, GenerationOptions options)
    {
        ITGenerator generator = GetGenerator(generatorName);
        if (generator != null)
        {
            return generator.Generate(options);
        }
        else
        {
            return null; // أو يمكنك طرح استثناء
        }
    }

    public void SaveCodeToFile(string generatorName, string filePath, GenerationOptions options)
    {
        ITGenerator generator = GetGenerator(generatorName);
        if (generator != null)
        {
            string generatedCode = generator.Generate(options);
            if (!string.IsNullOrEmpty(generatedCode))
            {
                File.WriteAllText(filePath, generatedCode);
                Console.WriteLine($"Generated code saved to {filePath}");
            }
            else
            {
                Console.WriteLine("No generated code to save.");
            }
        }
    }
}
