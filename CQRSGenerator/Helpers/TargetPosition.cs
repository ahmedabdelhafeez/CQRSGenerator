
using System.IO;

namespace CQRSGenerator.Helpers;

public class TargetPosition
{
    public Project Project { get; init; }
    public string TargetPath { get; init; }
    public string RootNS { get; init; }
    public string Entity { get; private set; }
    public CreateTypes Type { get; private set; }
    public string PluralizedEntity { get; private set; } 
    public string FolderPath { get; private set; }
    public string FolderNS { get; private set; }

    public void SetEntity(string entity,CreateTypes type)
    {
        Entity = entity;
        if(type==CreateTypes.CQRS)
        {
            PluralizedEntity = ProjectHelpers.Pluralize(Entity);
            FolderPath = Path.Combine(TargetPath, PluralizedEntity);
            FolderNS = RootNS + "." + PluralizedEntity;
        }
        else if(type==CreateTypes.Command)
        {
            Entity += "Command";
            FolderPath = Path.Combine(TargetPath, Entity);
            FolderNS = RootNS + "." + Entity;
        }
        else if (type == CreateTypes.Query)
        {
            Entity += "Query";
            FolderPath = Path.Combine(TargetPath, Entity);
            FolderNS = RootNS + "." + Entity;
        }
    }
}
