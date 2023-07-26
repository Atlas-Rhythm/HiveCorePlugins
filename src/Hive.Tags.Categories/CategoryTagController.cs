using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Hive.Tags.Categories;

[ApiController]
[Route("api/categories/")]
public class CategoryTagController : ControllerBase
{
    private readonly CategoryOptions _categoryOptions;

    public CategoryTagController(CategoryOptions categoryOptions) => _categoryOptions = categoryOptions;

    [HttpGet]
    public ActionResult<IEnumerable<string>> GetValidCategories() => Ok(_categoryOptions.Categories);
}
