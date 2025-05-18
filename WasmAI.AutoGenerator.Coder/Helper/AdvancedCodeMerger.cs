using GenerativeAI;
using GenerativeAI.Types;

public class GeminiCodeService
{
    private readonly GoogleAi _googleAI;
    private readonly GenerativeModel _generativeModel;
    private readonly List<Content> _contents;

    public GeminiCodeService(string apiKey= "AIzaSyALy9VxGquNEppI_YtSjPbKK06JyKALnT4")
    {
        _googleAI = new GoogleAi(apiKey);
        _generativeModel = _googleAI.CreateGenerativeModel("models/gemini-1.5-flash");

        string instructionText = @"You are a C# code generation expert. 
Return only raw C# code, with no extra text, symbols, code fences (such as ```), or explanations.
Your response must be pure C# code only.";

        _contents = new List<Content>()
        {
            new Content()
            {
                Role = "user",
                Parts = new List<Part>()
                {
                    new Part() { Text = instructionText }
                }
            }
        };
    }


    private string ExtractNotes(string code)
    {
        var notes = new List<string>();
        foreach (var line in code.Split('\n'))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("//&&") && trimmed.EndsWith("//"))
            {
                notes.Add(trimmed.Substring(4, trimmed.Length - 6).Trim());
            }
        }
        return string.Join("\n", notes);
    }

    public async Task<string> MergeCodesAsync2(string code1, string code2)
    {
        string notes1 = ExtractNotes(code1);
        string notes2 = ExtractNotes(code2);

        string notesCombined = string.Join("\n", new[] { notes1, notes2 }.Where(n => !string.IsNullOrWhiteSpace(n)));

        string prompt = string.IsNullOrWhiteSpace(notesCombined)
            ? @$"
You will be given two separate C# code snippets.
Your task is to intelligently merge them into a single clean, optimized, and functional C# code block.
Do not repeat duplicate functionality.
Preserve all important logic from both.
Only return the final merged code — no extra text or symbols.

First Code:
{code1}

Second Code:
{code2}"
            : @$"
You will be given two separate C# code snippets and additional notes embedded inside special comment markers.
//&&...//. These notes are important and should guide how the code should be merged.

Your task is to intelligently merge the codes into a single clean, optimized, and functional C# code block.
Follow the instructions provided inside the notes.
Do not repeat duplicate functionality. Preserve important logic from both.
Your task is to intelligently merge the codes into a single clean, optimized, and functional C# code block.

Important rules:
- If two methods have the same route attribute (e.g., [HttpGet(""{{id}}"", Name = ""..."")]), prefer the one from the **second code** (newer).
- Follow all instructions provided inside the notes.
- Do not repeat duplicate functionality.
- Preserve important logic from both codes.
- Only return the final merged code — no extra text or symbols.
Only return the final merged code — no extra text or symbols.

Developer Notes:
{notesCombined}

First Code:
{code1}

Second Code:
{code2}";

        _contents.Add(new Content()
        {
            Role = "user",
            Parts = new List<Part>()
        {
            new Part() { Text = prompt }
        }
        });

        var chatSession = _generativeModel.StartChat(_contents);
        var response = await chatSession.GenerateContentAsync("");
        return response.Text;
    }

    public async Task<string> GenerateCodeAsync(string codeRequest)
    {
        _contents.Add(new Content()
        {
            Role = "user",
            Parts = new List<Part>()
            {
                new Part() { Text = codeRequest }
            }
        });

        var chatSession = _generativeModel.StartChat(_contents);
        var response = await chatSession.GenerateContentAsync("");
        return response.Text;
    }

    public async Task<string> MergeCodesAsync(string code1, string code2)
    {
        _contents.Add(new Content()
        {
            Role = "user",
            Parts = new List<Part>()
            {
                new Part()
                {
                    Text = $@"
You will be given two separate C# code snippets.
Your task is to intelligently merge them into a single clean, optimized, and functional C# code block.
Do not repeat duplicate functionality.
Preserve all important logic from both.
Only return the final merged code — no extra text or symbols.

First Code:
{code1}

Second Code:
{code2}"
                }
            }
        });

        var chatSession = _generativeModel.StartChat(_contents);
        var response = await chatSession.GenerateContentAsync("");
        string txt = response.Text;
        if (txt.Contains("```csharp"))
        {
            txt = txt.Replace("```csharp", string.Empty);
        }
        if (txt.Contains("```"))
        {
            txt = txt.Replace("```", string.Empty);
        }


        return txt;
    }
}
