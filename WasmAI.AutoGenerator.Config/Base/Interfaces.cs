

namespace AutoGenerator
{
   
    public interface ITBase
    {
      
    }

    public interface ITModel : ITBase { }


    public interface ITDso : ITBase { }

    public interface ITDto : ITBase { }

    public interface ITShareDto : ITDto { }

    public interface ITBuildDto : ITDto { }

    public interface ITVM : ITBase { }

    public interface ITTransient : ITBase { }

    public interface ITSingleton : ITBase { }

    public interface ITScope : ITBase { }

    public interface ITRepository : ITBase { }

    public interface ITBaseRepository : ITRepository { }

    public interface ITBuildRepository : ITRepository { }

    public interface ITBaseShareRepository : ITRepository, ITScope { }

    public interface ITService : ITBase,ITScope { }

    public interface ITBuilder : ITBase, ITScope { }


    public interface ITBaseService : ITService { }



    public interface ITranslationData
    {
        Dictionary<string, string>? Value { get; set; }


        string? ToFilter(string? lg);
    }


    public interface ITAutoDbContext
    {

    }

    public interface ITUser
    {

    }
     
    public interface ITRole
    {

    }

    public interface ITClaimsHelper
    {

    }




    }

