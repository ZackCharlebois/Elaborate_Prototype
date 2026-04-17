/// <summary>
/// Don't specifically need anything here other than the fact it's persistent.
/// It is used to keep one main object which is never killed, with sub-systems as children.
/// </summary>
public class SystemsPersistent : SingletonPersistent<SystemsPersistent>
{
    
}