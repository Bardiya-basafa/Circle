namespace Domain.ViewModels.Home;

using Microsoft.AspNetCore.Http;


public class PostVM {
    public string Conent { get; set; }
    public IFormFile Image { get; set; }
}
