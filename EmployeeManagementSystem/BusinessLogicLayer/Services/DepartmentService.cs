﻿using AutoMapper;
using BusinessLogicLayer.Interface;
using BusinessLogicLayer.Models;
using DataAccessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;
        public DepartmentService(IDepartmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public void Create(Department department)
        {
            _repository.Create(_mapper.Map<DataAccessLayer.Entities.Department>(department));
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public List<Department> GetAll()
        {
            List<Department> departments = _mapper.Map<List<Department>>(_repository.GetAll());
            return departments;

        }

        public Department GetById(int id)
        {
            Department department = _mapper.Map<Department>(_repository.GetById(id));    
            return department;
        }

        public void Update(Department department)
        {
            _repository.Update(_mapper.Map<DataAccessLayer.Entities.Department>(department));
        }
    }
}
