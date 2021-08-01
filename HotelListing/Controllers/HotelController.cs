using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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

        [Authorize]
        [HttpGet("{id:int}")]
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

    }
}
