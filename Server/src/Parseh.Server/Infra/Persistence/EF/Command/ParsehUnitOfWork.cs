using Parseh.Server.Core.Contract.Infra.Persistence.Command;

namespace Parseh.Server.Infra.Persistence.EF.Command;

public sealed class ParsehUnitOfWork(ParsehCommandDbStore db)
    : UnitOfWork<ParsehCommandDbStore>(db), IParsehUnitOfWork
{

}
