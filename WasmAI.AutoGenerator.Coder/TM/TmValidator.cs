using AutoGenerator.ApiFolder;

namespace AutoGenerator.TM
{

    public class TmValidators
    {


        public static string GetTmConditionChecker(string classNameServiceTM, TmOptions options = null)
        {
            return @$"
 public class ConditionChecker :BaseConditionChecker, IConditionChecker
    {{
        private readonly ITFactoryInjector _injector;

        public ITFactoryInjector Injector => _injector;
        public ConditionChecker(ITFactoryInjector injector) : base()
        {{
        }}

        // الدوال السابقة تبقى كما هي

     
    }}

";



        }


        public static string GetTmIConditionChecker(string classNameServiceTM, TmOptions options = null)
        {
            return @$"
    public interface IConditionChecker: IBaseConditionChecker
    {{
   

        public ITFactoryInjector Injector {{ get; }}



    }}

";



        }


        public static string GetTmIValidatorContext(string classNameServiceTM, TmOptions options = null)
        {
            return @$"
      public abstract class ValidatorContext<TContext, EValidator> : BaseValidatorContext<TContext, EValidator>, ITValidator
       where TContext : class
       where EValidator : Enum
       
    {{
        protected readonly ITFactoryInjector _injector;


        public ValidatorContext(IConditionChecker checker) : base(checker)
        {{
            _injector= checker.Injector;
        }}
         
        
        protected virtual async Task<TContext?>  FinModel(string? id)
        {{
            

            var _model = await _injector.Context.Set<TContext>().FindAsync(id);
            return _model;



        }}


        protected override Task<TContext?> GetModel(string? id)
        {{
            
            return FinModel(id);
        }}


         
    }}




";



        }
        public static string GetTmITFactoryInjector(string classNameServiceTM, TmOptions options = null)
        {
            return @$"
     public interface ITFactoryInjector: ITBaseFactoryInjector
    {{

  
    public  {ApiFolderInfo.TypeContext.Name} Context {{ get; }}


    }}
";



        }


        public static string GetTmTFactoryInjector(string classNameServiceTM, TmOptions options = null)
        {
            return @$"
    public class TFactoryInjector : TBaseFactoryInjector, ITFactoryInjector
   {{
    
       private readonly {ApiFolderInfo.TypeContext.Name} _context;

       public TFactoryInjector(IMapper mapper, IAutoNotifier notifier,{ApiFolderInfo.TypeContext.Name} context) : base(mapper, notifier)
       {{
           _context = context;
       }}

       public {ApiFolderInfo.TypeContext.Name} Context => _context;
       // يمكنك حقن اي طبقة

   }}

";



        }


        public static string GetTmConfigValidator(string classNameServiceTM, TmOptions options = null)
        {
            return @$"
     public  static class ConfigValidator
    {{
        public static IServiceCollection AddAutoValidator(this IServiceCollection serviceCollection)
        {{


            Assembly? assembly =Assembly.GetExecutingAssembly();

            serviceCollection.AddScoped<ITFactoryInjector, TFactoryInjector>();
            serviceCollection.AddScoped<IConditionChecker, ConditionChecker>(pro =>
            {{
                var injctor = pro.GetRequiredService<ITFactoryInjector>();

                var checker= new ConditionChecker(injctor);


                BaseConfigValidator.Register(checker, assembly);

                return checker;

            }});

          return serviceCollection;



        }}
     
    }}

";



        }

        public static string GetTmValidator(string classNameValidatorTM, TmOptions options = null)
        {
            return @$"
    

    public class {classNameValidatorTM}Validator : BaseValidator<{classNameValidatorTM}ResponseFilterDso, {classNameValidatorTM}ValidatorStates>, ITValidator
    {{

    
        
        public {classNameValidatorTM}Validator(IConditionChecker checker) : base(checker)
        {{

           
        }}
        protected override void InitializeConditions()
        {{
            _provider.Register(
                {classNameValidatorTM}ValidatorStates.IsActive,
                new LambdaCondition<{classNameValidatorTM}ResponseFilterDso>(
                    nameof({classNameValidatorTM}ValidatorStates.IsActive),

                    context => IsActive(context),
                    ""{classNameValidatorTM} is not active""
                )
            );



            
        





        }}



        private bool IsActive({classNameValidatorTM}ResponseFilterDso context)
        {{
            if (context!=null){{
                return true;
            }}
            return false;
        }}

      

    }}

      //
       
     //  Base
     public enum {classNameValidatorTM}ValidatorStates //
    {{
        IsActive,
        IsFull,
        IsValid,
        
    //
    }}

 

";



        }



        public static string GetTmValidatorContext(string classNameValidatorTM, TmOptions options = null)
        {
            return @$"
    

    public class {classNameValidatorTM}ValidatorContext : ValidatorContext<{classNameValidatorTM}, {classNameValidatorTM}ValidatorStates>, ITValidator
    {{

    
        
        public {classNameValidatorTM}ValidatorContext(IConditionChecker checker) : base(checker)
        {{

           
        }}
        protected override void InitializeConditions()
        {{
           
     

        }}



      

      

    }}

      //
       
     //  Base
     public enum {classNameValidatorTM}ValidatorStates //
    {{
        IsActive,
        IsFull,
        IsValid,
        
    //
    }}

 

";



        }





        public static string GetTmRoleVoidator(TmOptions options = null)
        {
            return @"You are a C# code generation expert specialized in creating validator classes.
Your task is to generate a new C# validator class strictly following the architectural template provided below.
Return ONLY the raw C# code for the validator class and its associated enum(s) and helper classes (like ServiceType if needed).
Do NOT include any extra text, explanations, comments outside of the generated code, or markdown formatting (```csharp / ```).

--- Validator Template Architecture ---

This is an example of the desired C# validator architecture. Replicate its structure, patterns, and conventions precisely for the new validator you are asked to generate.

// Example Enum defining validation states
public enum ServiceValidatorStates
{
    IsFound = 6200,
    IsFull,
    HasName,
    HasAbsolutePath,
    IsCreateSpace,
    IsDashboard,
    HasToken,
    HasValidModelAi,
    HasMethods,
    HasRequests,
    IsLinkedToUsers,
    HasId,
    HasModelAi,
    HasLinkedUsers,
    IsServiceModel,
    IsServiceIdsEmpty,
    IsInUserClaims,
    IsIn,
}

// Example Helper class for constant values used in validation attributes
public class ServiceType
{
    public const string Dash = ""dashboard"";
    public const string Space = ""createspace"";
    public const string Service = ""service"";
}

// The main Validator class structure to follow
// It inherits from ValidatorContext<TModel, TState> and implements ITValidator
// It uses [RegisterConditionValidator] attributes to link states to methods
// It includes a private caching field (_service) and overrides GetModel
public class ServiceValidator : ValidatorContext<Service, ServiceValidatorStates>
{
    private Service? _service; // Example caching field

    public ServiceValidator(IConditionChecker checker) : base(checker)
    {
        // _checker field is available via the base class for cross-validation
        // Assume _injector is also available via base class or DI if needed
    }

    protected override void InitializeConditions()
    {
        // --- IMPORTANT INSTRUCTION ---
        // THIS METHOD MUST REMAIN EMPTY or contain only a base.InitializeConditions() call if necessary.
        // Do NOT add explicit RegisterCondition(...) calls here.
        // Conditions are registered AUTOMATICALLY by the framework based on the [RegisterConditionValidator] attributes.
        // -----------------------------
    }

    // --- Validation Function Signature and DataFilter Explanation ---

    // ALL validation methods MUST follow this exact signature pattern:
    // private async Task<ConditionResult> MethodName(DataFilter<TProp, TModel> f)

    // Explanation of the DataFilter<TProp, TModel> parameter (conventionally named 'f'):
    // This object carries all necessary context into a validation method.
    // - TProp: The TYPE of the SPECIFIC piece of data or comparison value relevant to THIS validation method.
    //   For example:
    //   - For checking a 'string Name', TProp is 'string'.
    //   - For checking a 'bool IsActive', TProp is 'bool'.
    //   - For checking if a collection 'ICollection<Item>' is not empty, TProp might be 'object'.
    //   - For checking if a property equals a specific list of strings, TProp is 'List<string>'.
    // - TModel: The TYPE of the ENTIRE model object being validated (e.g., Service, User, Product). This is consistent for all validation methods within one ValidatorContext class.

    // Key properties of the 'f' (DataFilter) object:
    // - f.Share (Type: TModel?): This is the MOST IMPORTANT property. It provides access to the FULL INSTANCE of the model object currently being validated. You use f.Share?.PropertyName to access the model's data.
    // - f.Value (Type: TProp?): This holds an OPTIONAL comparison value passed INTO the validation method specifically for this rule. Its type matches TProp. Used when the validation isn't just a simple check on the property itself (like NotNullOrWhitespace) but involves comparing the property's value to something external.
    // - f.Id (Type: string?): The ID associated with the validation request, often the ID of the model object.
    //   - f.Name (Type: string?): An optional name or key associated with the validation request context.

    // Return Type: Validation methods MUST return Task<ConditionResult>.
    // - ConditionResult.ToSuccessAsync(result): Use for successful validation, passing relevant data (often f.Share or the validated property value) in 'result'.
    // - ConditionResult.ToFailureAsync(message) or ConditionResult.ToFailureAsync(result, message): Use for failed validation, providing an error message.

    // Use of Framework Components:
    // - await _checker.CheckAndResultAsync(...): Available via the base class. Use for performing CROSS-VALIDATION checks by triggering other states/validators.
    // - _injector: If available via base class or DI, use to access application-specific services or context (like user claims or database context as seen in ServiceValidator examples).

    // --- Example Validation Functions (from ServiceValidator) ---
    // Replicate the structure, signatures, and parameter usage (f.Share, f.Value, f.Id, f.Name) as shown in these examples for the new validator.

    [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsFound, ""Service is not found"")]
    private Task<ConditionResult> ValidateId(DataFilter<string, Service> f)
    { // Checks f.Share.Id
        bool valid = !string.IsNullOrWhiteSpace(f.Share?.Id);
        return valid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync(""Service is not found"");
    }

    [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.HasName, ""Name is required"")]
    private Task<ConditionResult> ValidateName(DataFilter<string, Service> f)
    { // Checks f.Share.Name
        bool valid = !string.IsNullOrWhiteSpace(f.Share?.Name);
        return valid ? ConditionResult.ToSuccessAsync(f.Share?.Name) : ConditionResult.ToFailureAsync(f.Share?.Name, ""Name is required"");
    }

    [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.HasValidUrl, ""URL is invalid or missing"")]
    private Task<ConditionResult> ValidateAbsolutePath(DataFilter<string, Service> f)
    { // Checks f.Share.AbsolutePath using Uri.TryCreate
        bool valid = Uri.IsWellFormedUriString(f.Share?.AbsolutePath, UriKind.Absolute);
        return valid ? ConditionResult.ToSuccessAsync(f.Share?.AbsolutePath) : ConditionResult.ToFailureAsync(f.Share?.AbsolutePath, ""AbsolutePath is invalid"");
    }

    [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.HasToken, ""Token cannot be empty if provided"")]
    private Task<ConditionResult> ValidateToken(DataFilter<string?, Service> f)
    { // Checks f.Share.Token (nullable string)
        var token = f.Share?.Token;
        bool valid = token == null || !string.IsNullOrWhiteSpace(token);
        return valid ? ConditionResult.ToSuccessAsync(token) : ConditionResult.ToFailureAsync(""Token cannot be empty if provided"");
    }


    [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.HasModelAi, ""Model AI is missing"")]
    private async Task<ConditionResult> ValidateModelAi(DataFilter<string, Service> f)
    { // Example using f.Share, f.Share.ModelAiId and _checker
        if (f.Share == null) return ConditionResult.ToFailure(null, ""Model AI is missing (Model is null)"");
        // Assumes ModelValidatorStates is accessible
        var res = await _checker.CheckAndResultAsync(ModelValidatorStates.HasService, f.Share.ModelAiId);
        if (res.Success == true)
        {
            // Optionally assign the result from the checker if it returned the related object
            // f.Share.ModelAi = (ModelAi?)res.Result; // Assuming ModelAi is the type returned by the checker
            return ConditionResult.ToSuccess(f.Share);
        }
        return ConditionResult.ToFailure(f.Share, res.Message ?? ""Related ModelAi check failed."");
    }

    [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.HasMethods, ""No methods defined for service"")]
    private Task<ConditionResult> ValidateMethods(DataFilter<string, Service> f)
    { // Example checking a collection property (f.Share.ServiceMethods) for Any()
        bool valid = f.Share?.ServiceMethods != null && f.Share.ServiceMethods.Any();
        return valid ? ConditionResult.ToSuccessAsync(f.Share?.ServiceMethods) : ConditionResult.ToFailureAsync(f.Share?.ServiceMethods, ""No methods defined for service"");
    }

    [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsInUserClaims, ""Service is not in user claims"")]
    private Task<ConditionResult> ValidateServiceInUserClaims(DataFilter<string, Service> f)
    { // Example using f.Id and _injector (if available)
        // bool valid = _injector.UserClaims.ServicesIds?.Contains(f.Id) ?? false; // Original logic using _injector
        bool valid = f.Share?.Id == f.Id; // Placeholder logic for AI example
        return valid ? ConditionResult.ToSuccessAsync(f.Id) : ConditionResult.ToFailureAsync(f.Id, ""Service is not in user claims"");
    }

    [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsServiceIdsEmpty, ""User has no services"")]
    private Task<ConditionResult> ValidateServiceIdsEmpty(DataFilter<bool> f)
    { // Example using _injector (if available) and returning boolean result
        // bool isEmpty = _injector.UserClaims.ServicesIds?.Count == 0; // Original logic using _injector
        bool isEmpty = false; // Placeholder logic for AI example
        return isEmpty ? ConditionResult.ToSuccessAsync(isEmpty) : ConditionResult.ToFailureAsync(isEmpty, ""User has services"");
    }


    [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsServiceModel, ""Not a valid service model"")]
    [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsDashboard, ""Not a valid service model"", Value = ServiceType.Dash)]
    [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsCreateSpace, ""Not a valid service model"", Value = ServiceType.Space)]
    private Task<ConditionResult> ValidateIsServiceType(DataFilter<string, Service> f)
    { // Example using f.Share, f.Value, f.Name, and potential lookup within the validator
        if (f.Share == null && f.Value == null && f.Name == null) return Task.FromResult(ConditionResult.ToError(""Both Name and Value are null""));
        if (f.Share != null) return Task.FromResult(new ConditionResult(f.Share.AbsolutePath.Equals(f.Name ?? f.Value, StringComparison.OrdinalIgnoreCase), f.Share, $""No service found for {f.Name ?? f.Value}.""));
        // Lookup logic using _injector (if available)
        // f.Share = _injector.Context.Services.FirstOrDefault(x => x.AbsolutePath.Contains(f.Name ?? f.Value!));
        // bool valid = f.Share != null;
        bool valid = false; // Placeholder logic for AI example
        return valid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToErrorAsync($""No service found for {f.Name ?? f.Value}."");
    }

    [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsIn, ""Not a valid service model"")]
    private Task<ConditionResult> IsServiceType(DataFilter<List<string>, Service> f)
    { // Example using f.Share, f.Value (List<string>), and potential lookup within the validator
        if (f.Share == null && f.Value == null) return Task.FromResult(ConditionResult.ToError(""Both Name and Value are null""));
        if (f.Share != null) return Task.FromResult(new ConditionResult(f.Value?.Contains(f.Share.AbsolutePath) ?? false, f.Share, $""No service found for {f.Value}."")); // Null check f.Value
        // Lookup logic using _injector (if available)
        // f.Share = _injector.Context.Services.FirstOrDefault(x => f.Value.Contains(x.AbsolutePath));
        // bool valid = f.Share != null;
         bool valid = false; // Placeholder logic for AI example
        return valid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToErrorAsync($""No service found for {f.Value}."");
    }

    // --- Helper Methods ---
    // Override the GetModel method for caching or fetching the model instance
    protected override async Task<Service?> GetModel(string? id)
    {
        if (_service != null && _service.Id == id)
            return _service;
        _service = await base.GetModel(id); // Assumes base.GetModel fetches the model (e.g., from DB/repo)
        return _service;
    }
}

--- End Validator Template Architecture ---

Now, generate a new C# Validator class based on the following description, strictly adhering to the architectural pattern, DataFilter usage (f.Share, f.Value, f.Id, f.Name), method signatures, attributes, and conventions shown in the ServiceValidator template example above. Pay close attention to the explanation of the DataFilter and ensure you use f.Share to access model properties.
";

        }


    }

}