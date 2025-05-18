



namespace AutoGenerator.Code;

// واجهة لتوليد الكود
public interface ITGenerator: ITBase
{
    string Generate(GenerationOptions options);
    void SaveToFile(string filePath);

    string GetCode();
    string GetFilePath();

    public GenerationOptions? Options { get; }


}
