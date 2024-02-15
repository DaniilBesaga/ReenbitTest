using Microsoft.AspNetCore.Mvc;
using ReenbitTest.Server.Interfaces;
using ReenbitTest.Server.Models;
using System.IO;

namespace ReenbitTest.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly I_ItemRepository _iItemRepository;
        public ItemController(I_ItemRepository iImageRepository)
        {
            _iItemRepository = iImageRepository;
        }
        [HttpPost]
        public async Task<ActionResult> Upload([FromForm] Item item)
        {
            try
            {
                if (item == null)
                    return BadRequest();

                await _iItemRepository.AddItemAsync(item);

                return Ok(item);
            }
            catch (Exception ex)
            {
                string err = ex.Message + "\n" + ex.StackTrace;
                return BadRequest(err);
            }
        }
        [HttpGet]
        [Route("filename")]
        public async Task<ActionResult> Download(string name)
        {
            try
            {
                var itemStream = await _iItemRepository.GetItemByNameAsync(name); 
                return File(itemStream, "document/docx", name);
            }
            catch (Exception ex)
            {
                string err = ex.Message + "\n" + ex.StackTrace;
                return BadRequest(err);
            }
        }
        [HttpDelete]
        [Route("filename")]
        public async Task<ActionResult> Delete(string name)
        {
            try
            {
                await _iItemRepository.DeleteItemAsync(name);
                return Ok(name);
            }
            catch (Exception ex)
            {
                string err = ex.Message + "\n" + ex.StackTrace;
                return BadRequest(err);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ListAllBlobs()
        {
            var blobs = await _iItemRepository.GetAllItemsAsync();
            return Ok(blobs);
        }


    }
}
