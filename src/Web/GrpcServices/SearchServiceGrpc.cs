using GrpcSearchServiceNamespace;
using Grpc;
using Core.Repositories;
using Grpc.Core;
using Mapster;


namespace Web.GrpcServices;

public class SearchServiceGrpc : SearchServiceProto.SearchServiceProtoBase
{
    
    public SearchServiceGrpc()
    {
        
    }
    
}