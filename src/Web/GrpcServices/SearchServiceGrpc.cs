using GrpcSearchServiceNamespace;
using Grpc;
using Core.Repositories;
using Grpc.Core;
using Mapster;


namespace Web.GrpcServices;

public class SearchServiceGrpc : SearchServiceProto.SearchServiceProtoBase
{
    SelectionRepository _selectionRepository;
    public SearchServiceGrpc(SelectionRepository selectionRepository)
    {
        _selectionRepository = selectionRepository;
    }
    public override async Task<AddFilmSelectionResponse> AddFilmSelection(AddFilmSelectionRequest request, ServerCallContext context)
    {
        var selectionRes = await _selectionRepository.CreateSelection(request.Films.ToArray(), request.Name);
        return selectionRes.Adapt<AddFilmSelectionResponse>();
    }
    public override async Task<IsSuccessResponse> DeleteFilmSelection(DeleteFilmSelectionRequest request, ServerCallContext context)
    {
        var res = await _selectionRepository.Delete(request.Id);

        return new IsSuccessResponse {Success = res};
    }

    public override async Task<IsSuccessResponse> UpdateSelectionFilmsCollection(UpdateSelectionFilmsCollectionRequest request, ServerCallContext context)
    {
        var res = await _selectionRepository.UpdateSelectionFilmCollection(request.Id, request.Films.ToList());
        
        return new IsSuccessResponse {Success = res};
    }

    public override async Task<IsSuccessResponse> UpdateSelectionName(UpdateSelectionNameRequest request, ServerCallContext context)
    {
        var res = await _selectionRepository.UpdateSelectionName(request.Id, request.Name);
        
        return new IsSuccessResponse {Success = res};
    }
}