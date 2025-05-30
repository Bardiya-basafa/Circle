namespace Services.Interfaces;

using Domain.Entities;


public interface IAdminService {

    Task<List<Report>> GetPostReports(int postId); 

}
