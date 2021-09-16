using System.Threading.Tasks;
using GarmoFamilyTree.Interfaces;
using GarmoFamilyTree.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GarmoFamilyTree.Controllers
{
  [ApiController]
  [Route("api/familytree")]
  public class FamilyTreeController : ControllerBase
  {
    public FamilyTreeController(IFamilyTreeService iFamilyTreeService)
    {
      _familyTreeService = iFamilyTreeService;
    }

    private readonly IFamilyTreeService _familyTreeService;
    
    /// <summary>
    /// Add a person
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///   POST /Person
    ///   {
    ///     "FirstName": "Per",
    ///     "LastName": "Hansen",
    ///     "PersonIdentifier": "25035514441"
    ///   }
    /// </remarks>
    /// <param name="person"></param>
    /// <returns></returns>
    /// <response code="201">Returns the person created</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("addPerson")]
    public async Task<IActionResult> AddPerson([FromBody] Person person)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var personAdded = await _familyTreeService.AddUpdatePerson(person);

      return Ok(personAdded);
    }

    /// <summary>
    /// Updates a person
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///   PUT /Person
    ///   {
    ///     "FirstName": "Per",
    ///     "LastName": "Hansen",
    ///     "PersonIdentifier": "25035514441"
    ///   }
    /// </remarks>
    /// <param name="person"></param>
    /// <returns></returns>
    /// <response code="201">Returns the updated person</response>
    [HttpPut("updatePerson")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePerson([FromBody] Person person)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var personUpdated = await _familyTreeService.AddUpdatePerson(person);
      return Ok(personUpdated);
    }

    /// <summary>
    /// Add a persons child
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///   POST /Person
    ///   {
    ///     "FirstName": "Per",
    ///     "LastName": "Hansen",
    ///     "PersonIdentifier": "25035514441"
    ///   }
    /// </remarks>
    /// <param name="person"></param>
    /// <returns></returns>
    /// <response code="201">Returns the updated person</response>
    [HttpPost("addPersonChild/{parentId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPersonChild(int parentId, [FromBody] Person person)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      await _familyTreeService.AddUpdatePersonChild(parentId, person);

      return Ok();
    }

    // GET api/familytree/5
    [HttpGet("{personIdentifier}")]
    public async Task<IActionResult> GetByPersonIdentifier(string personIdentifier)
    {
      var familyTree = await _familyTreeService.GetFamilyTree(personIdentifier);
      if (familyTree == null)
      {
        return NotFound($"No person found with person identifier '{personIdentifier}'!");
      }
      return Ok(familyTree);
    }
  }
}
