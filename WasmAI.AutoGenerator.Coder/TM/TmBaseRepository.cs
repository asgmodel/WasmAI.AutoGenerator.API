using AutoGenerator.ApiFolder;

namespace AutoGenerator.TM
{

    public  class TmBaseRepository
    {


        public static string GetTmBaseRepository(string nameShareTM,bool isBPR=false ,TmOptions options = null)
        {

            if(!ApiFolderInfo.IsBPR) 
            return @$"
 /// <summary>
 /// {nameShareTM} class for ShareRepository.
 /// </summary>
        public sealed class BaseRepository<T> : TBaseRepository<{ApiFolderInfo.TypeIdentityUser.Name}, IdentityRole, string, T>, IBaseRepository<T> where T : class
    {{
        public BaseRepository({ApiFolderInfo.TypeContext.Name} db, ILoggerFactory logger) : base(db, logger)
        {{
        }}
    }}

";






            return @$"


   public sealed class BaseBPRRepository<T> : TBaseBPRRepository<{ApiFolderInfo.TypeIdentityUser.Name}, IdentityRole, string, T>,IBPRLayer<T,T>, IBaseRepository<T> where T : class
   {{
       public BaseBPRRepository({ApiFolderInfo.TypeContext.Name} db, ILoggerFactory logger) : base(db, logger)
       {{
       }}

      
   }}

";
        }


        public static string GetTmBaseBuilderRepository(string nameShareTM, bool isBPR = false, TmOptions options = null)
        {

            if (!ApiFolderInfo.IsBPR)
                return @$"
 /// <summary>
 /// {nameShareTM} class for ShareRepository.
 /// </summary>
  public abstract class BaseBuilderRepository<TModel, TBuildRequestDto, TBuildResponseDto> : TBaseBuilderRepository<TModel, TBuildRequestDto, TBuildResponseDto>, IBaseBuilderRepository<TBuildRequestDto, TBuildResponseDto>, ITBuildRepository
    where TModel : class
    where TBuildRequestDto : class
    where TBuildResponseDto : class

 {{
     public BaseBuilderRepository({ApiFolderInfo.TypeContext.Name} context, IMapper mapper, ILoggerFactory logger) : base(new BaseRepository<TModel>(context, logger), mapper, logger)
     {{
     }}





 }}

";




            return $@"

                  /// <summary>
    /// BaseRepository class for ShareRepository.
    /// </summary>
    public abstract class BaseBuilderRepository<TModel, TBuildRequestDto, TBuildResponseDto> :
                    BaseBPRLayer<TBuildRequestDto, TBuildResponseDto, TModel, TModel, ITBase, ITModel>, 
                    IBPRLayer<TBuildRequestDto, TBuildResponseDto>, ITBuildRepository, IBaseBuilderRepository<TBuildRequestDto, TBuildResponseDto>
                    where TModel : class where 
                    TBuildRequestDto : class
                    where TBuildResponseDto : class
   




    

                {{

                     
                    protected BaseBuilderRepository({ApiFolderInfo.TypeContext.Name} dbContext, IMapper mapper, ILoggerFactory logger):this(dbContext, mapper, logger, new BaseBPRRepository<TModel> (dbContext, logger))
                    {{

                       

                    }}

                    protected BaseBuilderRepository({ApiFolderInfo.TypeContext.Name} dbContext, IMapper mapper, ILoggerFactory logger, BaseBPRRepository<TModel> bpr) : base(mapper, logger, bpr)
                    {{
                    }}

                  

            }}
";
        }

    }
}