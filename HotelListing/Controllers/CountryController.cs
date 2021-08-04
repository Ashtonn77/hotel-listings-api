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
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;


        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {

            try
            {

                var countries = await _unitOfWork.Countries.GetAll();
                var results = _mapper.Map<IList<CountryDto>>(countries);
                return Ok(results);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in {nameof(GetCountries)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error. Please try again later");

            }

        }


        [HttpGet("{id:int}", Name = "GetCountry")]
        public async Task<IActionResult> GetCountry(int id)
        {

            try
            {

                var country = await _unitOfWork.Countries.Get(e => e.Id == id, new List<string>{ "Hotels" });
                var results = _mapper.Map<CountryDto>(country);
                return Ok(results);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in {nameof(GetCountry)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error. Please try again later");

            }

        }



        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDto countryDto)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogInformation($"Invalid post attempt in {nameof(CreateCountry)}");
                return BadRequest(ModelState);
            }

              
            try
            {

                var country = _mapper.Map<Country>(countryDto);
                await _unitOfWork.Countries.Insert(country);
                await _unitOfWork.Save();

                return CreatedAtRoute("GetCountry", new { id = country.Id }, country);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in {nameof(CreateCountry)}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Internal server error. Please try again later");
            }


        }



        [Authorize(Roles = "Administrator")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDto countryDto)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogInformation($"Invalid update attempt in {nameof(UpdateCountry)}");
                return BadRequest(ModelState);
            }


            try
            {

                var country = await _unitOfWork.Countries.Get(q => q.Id == id);

                if (country == null)
                {
                    _logger.LogInformation($"Invalid update attempt in {nameof(UpdateCountry)}");
                    return BadRequest("Submitted data is invalid");
                }

                _mapper.Map(countryDto, country);
                _unitOfWork.Countries.Update(country);
                await _unitOfWork.Save();

                return NoContent();


            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in {nameof(UpdateCountry)}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Internal server error. Please try again later");
            }


        }


       // [Authorize(Roles = "Administrator")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCountry(int id)
        {

            if (id < 1)
            {
                _logger.LogInformation($"Invalid delete attempt in {nameof(DeleteCountry)}");
                return BadRequest(ModelState);
            }


            try
            {

                var country = await _unitOfWork.Countries.Get(q => q.Id == id);
                if (country == null)
                {
                    _logger.LogInformation($"Invalid delete attempt in {nameof(DeleteCountry)}");
                    return BadRequest("Submitted data is invalid");
                }

                await _unitOfWork.Countries.Delete(id);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in {nameof(DeleteCountry)}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Internal server error. Please try again later");
            }


        }


    }
}
