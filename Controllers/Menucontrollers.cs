using System.Collections.Generic;
using AutoMapper;
using hobnobReact.Data;
using hobnobReact.Models;
using Microsoft.AspNetCore.Mvc;
using hobnobReact.Dtos;
using Microsoft.AspNetCore.JsonPatch;

namespace Menucontrollers.Controllers
{
    [Route("api/menus")]
    [ApiController]
    public class Menucontrollers : ControllerBase
    {
        private readonly IhobnobRepo _repository;
        private readonly IMapper _mapper;
        public Menucontrollers(IhobnobRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;

        }

        [HttpGet]
        public ActionResult<IEnumerable<Menu>> GetAllMenus()
        {
            var menuitems = _repository.GetAllmenus();
            return Ok(menuitems);
        }

        [HttpGet("{id}", Name = "GetMenuById")]
        public ActionResult<Menu> GetMenuById(int id)
        {
            var menuitems = _repository.GetMenuById(id);
            if (menuitems != null)
            {
                return Ok(menuitems);
            }
            return NotFound();
        }
        //POST api/ menu
        [HttpPost]
        public ActionResult<MenuReadDto> createMenu(MenuCreateDto menuCreateDto)
        {
            //source -> Target
            var menuModel = _mapper.Map<Menu>(menuCreateDto);// from the Dto(source User input ) to target Dto or profile.
            _repository.CreateMenu(menuModel);
            _repository.SaveChanges();
            // var MenuReadDto = _mapper.Map<MenuReadDto>(menuModel);// it need not to display filedname;
            //return Ok(menuModel);
            return CreatedAtRoute(nameof(GetMenuById), new { Id = menuModel });//,MenuReadDto);

        }
        //Put api Menus/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateMenu(int id, MenuUpdateDto menuUpdateDto)
        {
            var menumeodelfromRepo = _repository.GetMenuById(id);
            if (menumeodelfromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(menuUpdateDto, menumeodelfromRepo);// updated from from context

            _repository.updateMenu(menumeodelfromRepo); // does call the repo 

            _repository.SaveChanges();

            return NoContent();
        }
        //Patch api/Menus/{id}
        [HttpPatch("{id}")]
        public ActionResult PartialMenuUpdate(int id, JsonPatchDocument<MenuUpdateDto> patchDoc)
        {
            var menumeodelfromRepo = _repository.GetMenuById(id);
            if (menumeodelfromRepo == null)
            {
                return NotFound();
            }
            var menuTopatch = _mapper.Map<MenuUpdateDto>(menumeodelfromRepo);
            patchDoc.ApplyTo(menuTopatch, ModelState);

            if(!TryValidateModel(menuTopatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(menuTopatch, menumeodelfromRepo);

            _repository.updateMenu(menumeodelfromRepo);

            _repository.SaveChanges();

            return NoContent();
        }
    

     //Delete api / Menu/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteMenu(int id) 
        {
          
            var menumodelfromRepo = _repository.GetMenuById(id);

                if (menumodelfromRepo == null)
                {
                    return NotFound();
                }  
        
            _repository.DeleteMenu(menumodelfromRepo);
            _repository.SaveChanges();
            return NoContent();
        }
    }
}