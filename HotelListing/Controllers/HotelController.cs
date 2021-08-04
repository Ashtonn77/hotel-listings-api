using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;


        public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetHotels()
        {

            try
            {

                var hotels = await _unitOfWork.Hotels.GetAll();
                var results = _mapper.Map<IList<HotelDto>>(hotels);
                return Ok(results);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in {nameof(GetHotels)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error. Please try again later");

            }

        }

        
        [HttpGet("{id:int}", Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotel(int id)
        {

            try
            {

                var hotel = await _unitOfWork.Hotels.Get(e => e.Id == id);
                var results = _mapper.Map<HotelDto>(hotel);
                return Ok(results);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in {nameof(GetHotel)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error. Please try again later");

            }

        }


        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDto hotelDto)
        {
           
            if (!ModelState.IsValid)
            {
                _logger.LogInformation($"Invalid post attempt in {nameof(CreateHotel)}");
                return BadRequest(ModelState);
            }


            try
            {

                var hotel = _mapper.Map<Hotel>(hotelDto);
                await _unitOfWork.Hotels.Insert(hotel);
                await _unitOfWork.Save();

                return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in {nameof(CreateHotel)}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Internal server error. Please try again later");
            }


        }




        [Authorize(Roles = "Administrator")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDto hotelHotelDto)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogInformation($"Invalid update attempt in {nameof(UpdateHotel)}");
                return BadRequest(ModelState);
            }


            try
            {

                var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);

                if (hotel == null)
                {
                    _logger.LogInformation($"Invalid update attempt in {nameof(UpdateHotel)}");
                    return BadRequest("Submitted data is invalid");
                }

                _mapper.Map(hotelHotelDto, hotel);
                _unitOfWork.Hotels.Update(hotel);
                await _unitOfWork.Save();

                return NoContent();


            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in {nameof(CreateHotel)}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Internal server error. Please try again later");
            }


        }




        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHotel(int id)
        {

            if (id < 1)
            {
                _logger.LogInformation($"Invalid delete attempt in {nameof(DeleteHotel)}");
                return BadRequest(ModelState);
            }


            try
            {

                var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
                if (hotel == null)
                {
                    _logger.LogInformation($"Invalid delete attempt in {nameof(DeleteHotel)}");
                    return BadRequest("Submitted data is invalid");
                }

                await _unitOfWork.Hotels.Delete(id);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in {nameof(DeleteHotel)}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Internal server error. Please try again later");
            }


        }




    }
}
