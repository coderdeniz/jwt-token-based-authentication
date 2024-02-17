using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyService
{
    public static class ObjectMapper
    {
        // Lazy loading: objectMapper içindeki elemanlar çağırdığım zaman yüklensin boşu boşuna bellekte yer işgal etmesin

        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration((cfg) =>
            {
                cfg.AddProfile<DtoMapper>();
            });

            return config.CreateMapper();
        });

        /// <summary>
        /// get methoduna lazy'nin value'sini alır
        /// </summary>
        public static IMapper Mapper => lazy.Value;
    }
}
