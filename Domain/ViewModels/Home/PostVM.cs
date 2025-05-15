namespace Domain.ViewModels.Home;

using Microsoft.AspNetCore.Http;


public class PostVM {

    public string Content { get; set; }

    public IFormFile Image { get; set; }

}
