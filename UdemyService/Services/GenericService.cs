using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UdemyCore.Models;
using UdemyCore.Repositories;
using UdemyCore.Services;
using UdemyShared.Dtos;

namespace UdemyService.Services
{
    public class GenericService<TEntity, TDto> : IServiceGeneric<TEntity, TDto>
        where TEntity : class where TDto : class
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _genericRepository;


        public GenericService(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        public async Task<Response<TDto>> AddAsync(TDto entity)
        {
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(entity);

            await _genericRepository.AddAsync(newEntity);

            await _unitOfWork.CommitAsync();

            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);

            return Response<TDto>.Success(newDto,StatusCodes.Status200OK);
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            var products = ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await _genericRepository.GetAllAsync());

            return Response<IEnumerable<TDto>>.Success(products, StatusCodes.Status200OK);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var product = await _genericRepository.GetByIdAsync(id);

            if (product == null)
            {
                return Response<TDto>.Fail("Id not found", StatusCodes.Status404NotFound, true);
            }

            return Response<TDto>.Success(ObjectMapper.Mapper.Map<TDto>(product), StatusCodes.Status200OK);
        }

        public async Task<Response<NoDataDto>> RemoveAsync(int id)
        {
            var isExistEntity = await _genericRepository.GetByIdAsync(id);

            if (isExistEntity == null)
            {
                return Response<NoDataDto>.Fail("ID not found", StatusCodes.Status404NotFound, true);
            }

            _genericRepository.Remove(isExistEntity);

            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(StatusCodes.Status204NoContent);
        }

        public async Task<Response<NoDataDto>> UpdateAsync(TDto entity, int id)
        {
            var isExistEntity = await _genericRepository.GetByIdAsync(id);

            if (isExistEntity == null)
            {
                return Response<NoDataDto>.Fail("ID not found", StatusCodes.Status404NotFound, true);
            }

            var updateEntity = ObjectMapper.Mapper.Map<TEntity>(entity);

            _genericRepository.Update(updateEntity);

            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(StatusCodes.Status204NoContent);
        }


        /// <summary>
        /// toList diyene kadar veritabanına yansımıyor yani memory'e çekilmiyor     
        /// </summary>
        public async Task<Response<IEnumerable<TDto>>> WhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var list = _genericRepository.Where(predicate);

            return Response<IEnumerable<TDto>>.Success(
                ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await list.ToListAsync()),
                StatusCodes.Status200OK);
        }
    }
}
