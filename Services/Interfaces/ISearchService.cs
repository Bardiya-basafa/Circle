namespace Services.Interfaces;

using Domain.DTO;


public interface ISearchService {

    Task<SearchResponse> Search(string searchString, int userId);

}
