using Domain.Entities.Base;
using Domain.Interfaces.Base;
using Domain.Models;
//using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace Domain.Repository.Base
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbset;

        public Repository(DbContext context)
        {
            _context = context;
            _dbset = _context.Set<T>();
        }

        private static readonly ParsingConfig _cfg = new() { IsCaseSensitive = false };

        public DbContext DbContext => _context;
        public DbSet<T> DbSet => _dbset;
        public IQueryable<T> Queryable => _dbset.AsQueryable();

        public IQueryable<T> Where(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = DbContext.Set<T>();

            foreach (var include in includes)
                query = query.Include(include);

            return query.Where(expression).AsNoTracking();
        }

        public async Task<int> CountAsync()
        {
            return await _dbset.AsNoTracking().CountAsync();
        }

        // Método CountAsync sin includes explícitos
        public async Task<int> CountAsync(string? searchTerm, List<string>? searchColumns, List<string>? filter, List<string>? filterOperator, List<string>? value)
        {
            return await CountAsync(searchTerm, searchColumns, filter, filterOperator, value, (string[]?)null);
        }

        // FUNCIONALIDAD PRINCIPAL
        // Método CountAsync con includes explícitos
        public async Task<int> CountAsync(
            string? searchTerm,
             List<string>? searchColumns,
             List<string>? filter,
             List<string>? filterOperator,
             List<string>? value,
             string[]? includes
             )
        {
            var query = _dbset.AsQueryable().Where(x => x.Activo == true);

            if (includes != null && includes.Length > 0)
            {
                foreach (var inc in includes)
                    query = query.Include(inc);
            }

            // Búsqueda rápida
            if (!string.IsNullOrEmpty(searchTerm) && searchColumns != null)
            {
                var parts = searchColumns
                    .Select(col => BuildFilterExpression(col, "like", searchTerm, true).filter)
                    .Where(f => !string.IsNullOrEmpty(f))
                    .ToList();

                if (parts.Count != 0)
                {
                    var searchFilter = string.Join(" OR ", parts!);
                    query = query.Where(searchFilter, searchTerm);
                }
            }

            // Filtros dinámicos
            if (filter != null && filterOperator != null && value != null)
            {
                for (int i = 0; i < filter.Count; i++)
                {
                    var (expr, val) = BuildFilterExpression(filter[i], filterOperator[i], value[i], true);
                    if (!string.IsNullOrEmpty(expr))
                        query = query.Where(expr!, val!);
                }
            }

            return await query.AsNoTracking().CountAsync();
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> expression)
        {
            return _dbset.AsNoTracking().CountAsync(expression);
        }

        // Método CountAsync con DinamicFilters sin includes
        public Task<int> CountAsync(DinamicFilters parameters)
        {
            return CountAsync(parameters.SearchTerm, parameters.SearchColumns, parameters.Filter, parameters.FilterOperator, parameters.FilterValue);
        }

        // Método CountAsync con DinamicFilters e includes
        public Task<int> CountAsync(DinamicFilters parameters, string[]? includes)
        {
            return CountAsync(parameters.SearchTerm, parameters.SearchColumns, parameters.Filter, parameters.FilterOperator, parameters.FilterValue, includes);
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _dbset.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> DeleteAsync(long id)
        {
            var entity = await _dbset.FindAsync(id) ?? throw new Exception("No existe el registro a eliminar");
            entity.Activo = false;
            entity.FechaEliminacion = DateTime.Now;
            entity.IpEliminacion = "";
            entity.IpRegistro = "";
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _dbset.FindAsync(id) != null;
        }

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
        {
            return _dbset.AsNoTracking().AnyAsync(expression);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbset.AsNoTracking().ToListAsync();
        }

        // Método GetAllAsync sin includes explícitos
        public async Task<List<T>> GetAllAsync(int? page, int? pageSize, string? sortColumn, string? sortOrder, string? searchTerm, List<string>? searchColumns, List<string>? filter, List<string>? filterOperator, List<string>? value)
        {
            return await GetAllAsync(page, pageSize, sortColumn, sortOrder, searchTerm, searchColumns, filter, filterOperator, value, null);
        }

        //-- FUNCIONALIDAD PRINCIPAL
        // Método GetAllAsync con includes explícitos 
        public async Task<List<T>> GetAllAsync(int? page, int? pageSize, string? sortColumn, string? sortOrder, string? searchTerm, List<string>? searchColumns, List<string>? filter, List<string>? filterOperator, List<string>? value, string[]? includes)
        {
            var query = _dbset.AsQueryable().Where(x => x.Activo == true);

            if (includes != null && includes.Length > 0)
            {
                foreach (var inc in includes)
                    query = query.Include(inc);
            }

            // Búsqueda rápida
            if (!string.IsNullOrEmpty(searchTerm) && searchColumns != null)
            {
                var parts = searchColumns
                    .Select(col => BuildFilterExpression(col, "like", searchTerm, true).filter)
                    .Where(f => !string.IsNullOrEmpty(f))
                    .ToList();

                if (parts.Any())
                {
                    var searchFilter = string.Join(" OR ", parts!);
                    query = query.Where(searchFilter, searchTerm);
                }
            }

            // Filtros dinámicos
            if (filter != null && filterOperator != null && value != null)
            {
                for (int i = 0; i < filter.Count; i++)
                {
                    var (expr, val) = BuildFilterExpression(filter[i], filterOperator[i], value[i], true);
                    if (!string.IsNullOrEmpty(expr))
                        query = query.Where(expr!, val!);
                }
            }

            // Ordenamiento dinámico
            if (!string.IsNullOrEmpty(sortColumn))
            {
                var orderExpr = BuildOrderExpression(sortColumn, sortOrder ?? string.Empty);
                query = query.OrderBy(orderExpr);
            }

            // Paginación
            if (page.HasValue && pageSize.HasValue)
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

            return await query.AsNoTracking().ToListAsync();
        }

        // Método GetAllAsync con DinamicFilters sin includes
        public Task<List<T>> GetAllAsync(DinamicFilters parameters)
        {
            return GetAllAsync(
                parameters.Page,
                parameters.PageSize,
                parameters.SortColumn,
                parameters.SortOrder,
                parameters.SearchTerm,
                parameters.SearchColumns,
                parameters.Filter,
                parameters.FilterOperator,
                parameters.FilterValue
                );
        }

        // Método GetAllAsync con DinamicFilters e includes
        public Task<List<T>> GetAllAsync(DinamicFilters parameters, string[]? includes)
        {
            return GetAllAsync(
                parameters.Page,
                parameters.PageSize,
                parameters.SortColumn,
                parameters.SortOrder,
                parameters.SearchTerm,
                parameters.SearchColumns,
                parameters.Filter,
                parameters.FilterOperator,
                parameters.FilterValue,
                includes
            );
        }

        // Método GetByIdAsync sin includes
        public async Task<T?> GetByIdAsync(long id)
        {
            return await _dbset.FindAsync(id);
        }

        // Método GetByIdAsync con includes
        public async Task<T?> GetByIdAsync(long id, string[]? includes)
        {
            // Primero obtenemos la entidad base usando FindAsync que funciona con la clave primaria
            // independientemente del nombre que tenga en la entidad
            var entity = await _dbset.FindAsync(id);

            if (entity == null)
                return null;

            // Si tenemos includes que cargar y la entidad existe
            if (includes != null && includes.Length > 0)
            {
                // Obtenemos todas las propiedades de clave primaria de la entidad
                var keyNames = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties
                    .Select(p => p.Name).ToList();

                if (keyNames == null || !keyNames.Any())
                {
                    // Si no podemos determinar las claves, simplemente retornamos la entidad sin includes
                    return entity;
                }

                // Desconectamos la entidad para poder recargarla con los includes
                _context.Entry(entity).State = EntityState.Detached;

                // Creamos un nuevo query con los includes
                var query = _dbset.AsQueryable();

                // Aplicamos los includes necesarios
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

                // Construimos un predicado dinámico para buscar por la clave primaria
                // Asumiendo que estamos trabajando con una sola clave primaria
                var parameter = Expression.Parameter(typeof(T), "e");
                var keyProperty = Expression.Property(parameter, keyNames.First());
                var keyValue = Expression.Constant(id);
                var equalExpr = Expression.Equal(keyProperty, keyValue);
                var lambda = Expression.Lambda<Func<T, bool>>(equalExpr, parameter);

                // Ejecutamos la consulta
                var result = await query.FirstOrDefaultAsync(lambda);

                if (result != null)
                    return result;
            }

            // Si no hay includes o no se pudo recargar, devolvemos la entidad original
            return entity;
        }

        public async Task<T> UpdateAsync(long? id, T entity)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var existingEntity = await GetByIdAsync(id.Value) ?? throw new Exception("Entity not found");

            // Guardar los valores importantes antes de la actualización
            bool activoOriginal = existingEntity.Activo;
            DateTime fechaRegistroOriginal = existingEntity.FechaRegistro;
            string? usuarioRegistroOriginal = existingEntity.UsuarioRegistro;

            // Actualizar la entidad con los nuevos valores
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);

            // Restaurar los valores importantes
            existingEntity.Activo = activoOriginal;
            existingEntity.FechaRegistro = fechaRegistroOriginal;
            existingEntity.UsuarioRegistro = usuarioRegistroOriginal;

            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdatePartialAsync(long id, object partialEntity)
        {
            if (partialEntity == null)
                throw new ArgumentNullException(nameof(partialEntity));

            // 1) Traemos la entidad existente
            var existing = await _dbset.FindAsync(id)
                         ?? throw new KeyNotFoundException($"No se encontró la entidad con id {id}");

            // 2) Preparamos el Entry para marcar propiedades
            var entry = _context.Entry(existing);

            // 3) Recorremos sólo las propiedades no-nulas del objeto parcial
            var props = partialEntity.GetType()
                            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                          .Where(p => p.GetValue(partialEntity) != null);

            foreach (var prop in props)
            {
                var propName = prop.Name;
                var newValue = prop.GetValue(partialEntity);

                // 3.1) Asignamos el nuevo valor a la propiedad de la entidad
                typeof(T).GetProperty(propName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase)
                         ?.SetValue(existing, newValue);

                // 3.2) Marcamos sólo esa propiedad como modificada
                entry.Property(propName).IsModified = true;
            }

            // 4) Siempre actualizamos la fecha de modificación
            var fechaModProp = nameof(BaseEntity.FechaModificacion);
            existing.FechaModificacion = DateTime.Now;
            entry.Property(fechaModProp).IsModified = true;

            // 5) Guardamos cambios
            await _context.SaveChangesAsync();
            return existing;
        }

        // Método para convertir expresiones lambda a strings para includes
        private string[] ConvertIncludePropertiesToStrings(params Expression<Func<T, object>>[] includeProperties)
        {
            if (includeProperties == null || includeProperties.Length == 0)
                return [];

            var includeStrings = new List<string>();

            foreach (var includeProperty in includeProperties)
            {
                string propertyPath = GetPropertyPath(includeProperty);
                if (!string.IsNullOrEmpty(propertyPath))
                {
                    includeStrings.Add(propertyPath);
                }
            }

            return [.. includeStrings];
        }

        // Método para extraer el path de propiedad de una expresión lambda
        private string GetPropertyPath(Expression<Func<T, object>> expression)
        {
            // Obtener el cuerpo de la expresión
            Expression body = expression.Body;

            // Si el cuerpo es una expresión unaria (común cuando se devuelve un tipo de valor)
            if (body is UnaryExpression unaryExpression)
            {
                body = unaryExpression.Operand;
            }

            // Obtener la cadena de miembros mediante el método auxiliar
            return Repository<T>.GetMemberPath(body);
        }



        // Implementación de GetAllAsync con expresiones lambda para includes
        public Task<List<T>> GetAllAsync(DinamicFilters parameters, params Expression<Func<T, object>>[] includeProperties)
        {
            string[] includeStrings = ConvertIncludePropertiesToStrings(includeProperties);
            return GetAllAsync(parameters, includeStrings);
        }

        // Implementación de GetAllAsync con sortProperty e includeProperties
        public Task<List<T>> GetAllAsync<TKey>(
            int? page, int? pageSize, Expression<Func<T, TKey>>? sortProperty, string? sortOrder,
            string? searchTerm, List<string>? searchColumns, List<string>? filter,
            List<string>? filterOperator, List<string>? value,
            params Expression<Func<T, object>>[] includeProperties)
        {
            string? sortColumn = null;

            if (sortProperty != null)
            {
                // Extraer el nombre de la propiedad de ordenamiento de la expresión
                if (sortProperty.Body is MemberExpression memberExpression)
                {
                    sortColumn = memberExpression.Member.Name;
                }
                else if (sortProperty.Body is UnaryExpression unaryExpression &&
                         unaryExpression.Operand is MemberExpression memberExpr)
                {
                    sortColumn = memberExpr.Member.Name;
                }
            }

            string[] includeStrings = ConvertIncludePropertiesToStrings(includeProperties);

            return GetAllAsync(page, pageSize, sortColumn, sortOrder, searchTerm,
                searchColumns, filter, filterOperator, value, includeStrings);
        }

        // Implementación de CountAsync con expresiones lambda para includes
        public Task<int> CountAsync(
            string? searchTerm, List<string>? searchColumns, List<string>? filter,
            List<string>? filterOperator, List<string>? value,
            params Expression<Func<T, object>>[] includeProperties)
        {
            string[] includeStrings = ConvertIncludePropertiesToStrings(includeProperties);
            return CountAsync(searchTerm, searchColumns, filter, filterOperator, value, includeStrings);
        }

        // Implementación de CountAsync con DinamicFilters e includeProperties
        public Task<int> CountAsync(DinamicFilters parameters, params Expression<Func<T, object>>[] includeProperties)
        {
            string[] includeStrings = ConvertIncludePropertiesToStrings(includeProperties);
            return CountAsync(parameters, includeStrings);
        }

        // Implementación de GetByIdAsync con expresiones lambda para includes
        public Task<T?> GetByIdAsync(long id, params Expression<Func<T, object>>[] includeProperties)
        {
            string[] includeStrings = ConvertIncludePropertiesToStrings(includeProperties);
            return GetByIdAsync(id, includeStrings);
        }

        // Método auxiliar para obtener la ruta de acceso de una expresión de miembro de forma recursiva
        private static string GetMemberPath(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                // Si la expresión de miembro tiene una expresión interna que no es un parámetro
                if (memberExpression.Expression != null && !(memberExpression.Expression is ParameterExpression))
                {
                    // Recursivamente obtener la ruta del miembro padre y concatenar
                    return Repository<T>.GetMemberPath(memberExpression.Expression) + "." + memberExpression.Member.Name;
                }

                // Si llegamos a un miembro que es directamente del parámetro
                return memberExpression.Member.Name;
            }

            return string.Empty;
        }

        // Método para normalizar las rutas de propiedades
        private string NormalizePropertyPath(string propertyPath, bool forSearch = true)
        {
            if (string.IsNullOrWhiteSpace(propertyPath))
                return propertyPath;

            var parts = propertyPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
            var normalizedParts = new List<string>();

            // 1) Comprobación temprana: si no encontramos el metadata de la entidad, devolvemos tal cual
            var entityType = _context.Model.FindEntityType(typeof(T));
            if (entityType == null)
                return propertyPath;

            // Ya podemos quitar los operadores null‑conditional (?.) porque entityType NO es nulo
            var properties = entityType.GetProperties()
                                       .ToDictionary(p => p.Name.ToLower(), p => p.Name, StringComparer.OrdinalIgnoreCase);
            var navigations = entityType.GetNavigations()
                                        .ToDictionary(n => n.Name.ToLower(), n => n.Name, StringComparer.OrdinalIgnoreCase);

            Type currentType = typeof(T);
            bool isCollection = false;
            // ahora la inicializamos con un valor seguro
            string collectionItem = "item";

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                var partLower = part.ToLower();
                string normalizedPart = part;

                if (i == 0)
                {
                    // Primera parte: chequeamos en propiedades o en navigations
                    if (properties.TryGetValue(partLower, out var propName))
                    {
                        normalizedPart = propName;
                        var propInfo = typeof(T).GetProperty(propName,
                            System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.IgnoreCase);

                        if (propInfo != null
                         && IsCollectionType(propInfo.PropertyType, out var elementType))
                        {
                            isCollection = true;
                            currentType = elementType!;
                            // forSearch + elemento intermedio → abrimos el Any(
                            if (forSearch && i < parts.Length - 1)
                            {
                                normalizedParts.Add($"{normalizedPart}.Any({collectionItem} => ");
                                continue;
                            }
                        }
                        else if (propInfo != null)
                        {
                            currentType = propInfo.PropertyType;
                        }
                    }
                    else if (navigations.TryGetValue(partLower, out var navName))
                    {
                        normalizedPart = navName;
                        var nav = entityType.FindNavigation(navName)!; // sabemos que existe

                        if (IsCollectionType(nav.ClrType, out var elementType))
                        {
                            isCollection = true;
                            currentType = elementType!;
                            if (forSearch && i < parts.Length - 1)
                            {
                                normalizedParts.Add($"{normalizedPart}.Any({collectionItem} => ");
                                continue;
                            }
                        }
                        else
                        {
                            currentType = nav.ClrType;
                        }
                    }
                }
                else if (isCollection && forSearch)
                {
                    // Si viene dentro de un Any(...), refrescamos el metadata
                    var nextEntityType = _context.Model.FindEntityType(currentType!);
                    if (nextEntityType == null)
                    {
                        normalizedParts.Add(part);
                        continue;
                    }

                    var nextProps = nextEntityType.GetProperties()
                                                  .ToDictionary(p => p.Name.ToLower(), p => p.Name, StringComparer.OrdinalIgnoreCase);
                    var nextNavs = nextEntityType.GetNavigations()
                                                  .ToDictionary(n => n.Name.ToLower(), n => n.Name, StringComparer.OrdinalIgnoreCase);

                    if (nextProps.TryGetValue(partLower, out var np))
                        normalizedPart = $"{collectionItem}.{np}";
                    else if (nextNavs.TryGetValue(partLower, out var nv))
                        normalizedPart = $"{collectionItem}.{nv}";
                    else
                        normalizedPart = $"{collectionItem}.{part}"; // Mantener la parte original si no se encuentra

                    // Si es la última parte, cerramos el paréntesis del Any
                    if (i == parts.Length - 1)
                        normalizedPart += ")";
                }
                else
                {
                    // Parte normal tras un . que no es colección
                    var nextEntityType = _context.Model.FindEntityType(currentType!);
                    if (nextEntityType != null)
                    {
                        var nextProps = nextEntityType.GetProperties()
                                                      .ToDictionary(p => p.Name.ToLower(), p => p.Name, StringComparer.OrdinalIgnoreCase);
                        var nextNavs = nextEntityType.GetNavigations()
                                                      .ToDictionary(n => n.Name.ToLower(), n => n.Name, StringComparer.OrdinalIgnoreCase);

                        if (nextProps.TryGetValue(partLower, out var np))
                            normalizedPart = np;
                        else if (nextNavs.TryGetValue(partLower, out var nv))
                        {
                            normalizedPart = nv;
                            var nav = nextEntityType.FindNavigation(nv)!;
                            if (IsCollectionType(nav.ClrType, out var elementType))
                            {
                                isCollection = true;
                                currentType = elementType!;
                                if (forSearch && i < parts.Length - 1)
                                {
                                    normalizedParts.Add($"{normalizedPart}.Any({collectionItem} => ");
                                    continue;
                                }
                            }
                            else
                            {
                                currentType = nav.ClrType;
                            }
                        }
                    }
                }

                normalizedParts.Add(normalizedPart);
            }

            return string.Join(".", normalizedParts);
        }

        // Método para determinar si un tipo es una colección y obtener el tipo de elemento
        private bool IsCollectionType(Type type, out Type? elementType)
        {
            elementType = null;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
            foreach (var iface in type.GetInterfaces())
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    elementType = iface.GetGenericArguments()[0];
                    return true;
                }
            }
            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }
            return false;
        }


        public async Task<T> UpdateAsync(T entity)
        {
            entity.FechaModificacion = DateTime.Now;
            _dbset.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        //public bool BulkUpdate(IEnumerable<T> entities)
        //{
        //    DbContext.BulkUpdate(entities, options =>
        //    {
        //        options.PropertiesToExclude = new List<string> { "UsuarioRegistro", "FechaRegistro", "IpRegistro", "UsuarioModificacion", "FechaModificacion", "IpModificacion" };
        //    });

        //    foreach (var obj in entities)
        //        DbContext.Entry(obj).State = EntityState.Detached;

        //    return true;
        //}
        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _dbset.AsQueryable().Where(expression);

            string[] includeStrings = ConvertIncludePropertiesToStrings(includeProperties);

            foreach (var include in includeStrings)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        // Nuevo método con soporte para ThenInclude
        public async Task<List<T>> GetAllAsync(DinamicFilters parameters, Func<IQueryable<T>, IQueryable<T>> includeFunc)
        {
            var query = _dbset.AsQueryable().Where(x => x.Activo == true);

            // ---- búsqueda rápida ----
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm) &&
                parameters.SearchColumns?.Any() == true)
            {
                var searchConditions = new List<string>();
                var searchValue = parameters.SearchTerm.ToLower();

                foreach (var column in parameters.SearchColumns)
                {
                    try
                    {
                        // Dividir la ruta de la propiedad en partes
                        var parts = column.Split('.');

                        // Si solo es una propiedad simple (no hay puntos)
                        if (parts.Length == 1)
                        {
                            var propertyName = GetActualPropertyName(typeof(T), parts[0]);
                            if (!string.IsNullOrEmpty(propertyName))
                            {
                                var propertyType = GetPropertyType(typeof(T), propertyName);

                                if (IsNumericType(propertyType))
                                {
                                    // Para campos numéricos, usar comparación de igualdad si el searchTerm es numérico
                                    if (decimal.TryParse(parameters.SearchTerm, out var numericValue))
                                    {
                                        searchConditions.Add($"{propertyName} == @1");
                                    }
                                    // Si no es numérico, saltar este campo
                                }
                                else if (IsStringType(propertyType))
                                {
                                    // Para campos de texto, usar Contains
                                    searchConditions.Add($"{propertyName}.Contains(@0)");
                                }
                            }
                            continue;
                        }

                        // Obtener el nombre real de la propiedad raíz
                        var rootProperty = GetActualPropertyName(typeof(T), parts[0]);
                        if (string.IsNullOrEmpty(rootProperty))
                        {
                            Console.WriteLine($"No se encontró la propiedad '{parts[0]}' en la entidad");
                            continue;
                        }

                        // Determinar si la raíz es una colección
                        bool isCollection = IsCollectionProperty(typeof(T), rootProperty);

                        if (isCollection)
                        {
                            // Es una colección (como Modelos)
                            var nestedPath = string.Join(".", parts.Skip(1));

                            if (string.IsNullOrEmpty(nestedPath))
                            {
                                Console.WriteLine($"No se especificó una propiedad para la colección '{rootProperty}'");
                                continue;
                            }

                            // Construir una expresión segura para la búsqueda en la colección (sin ToString)
                            searchConditions.Add($"{rootProperty}.Any(item => item.{nestedPath}.Contains(@0))");
                        }
                        else
                        {
                            // Es una navegación simple (como Equipo)
                            var fullPath = rootProperty;

                            for (int i = 1; i < parts.Length; i++)
                            {
                                var propName = GetActualPropertyName(GetNavigationPropertyType(typeof(T), string.Join(".", parts.Take(i))), parts[i]);
                                if (string.IsNullOrEmpty(propName))
                                {
                                    Console.WriteLine($"No se encontró la propiedad '{parts[i]}' en la ruta '{string.Join(".", parts.Take(i))}'");
                                    fullPath = null;
                                    break;
                                }
                                fullPath += "." + propName;
                            }

                            if (!string.IsNullOrEmpty(fullPath))
                            {
                                // Usar EF.Functions.Like en lugar de ToString().Contains
                                searchConditions.Add($"{fullPath}.Contains(@0)");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar columna de búsqueda '{column}': {ex.Message}");
                    }
                }

                if (searchConditions.Any())
                {
                    var searchExpression = string.Join(" OR ", searchConditions);
                    Console.WriteLine($"Expresión de búsqueda: {searchExpression}");

                    try
                    {
                        if (decimal.TryParse(parameters.SearchTerm, out var numericSearchValue))
                        {
                            // Si es numérico, pasar tanto string como numeric
                            query = query.Where(_cfg, searchExpression, searchValue, numericSearchValue);
                        }
                        else
                        {
                            // Solo string
                            query = query.Where(_cfg, searchExpression, searchValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al aplicar búsqueda: {ex.Message}");  
                    }
                }
            }

            // Aplicar filtros dinámicos
            if (parameters.Filter != null && parameters.FilterOperator != null && parameters.FilterValue != null &&
                parameters.Filter.Count > 0 && parameters.Filter.Count == parameters.FilterOperator.Count &&
                parameters.Filter.Count == parameters.FilterValue.Count)
            {
                for (int i = 0; i < parameters.Filter.Count; i++)
                {
                    try
                    {
                        var column = parameters.Filter[i];
                        var op = parameters.FilterOperator[i].ToLower();
                        var val = parameters.FilterValue[i];

                        // Dividir la ruta de la propiedad en partes
                        var parts = column.Split('.');

                        // Si solo es una propiedad simple (no hay puntos)
                        if (parts.Length == 1)
                        {
                            var propertyName = GetActualPropertyName(typeof(T), parts[0]);
                            if (!string.IsNullOrEmpty(propertyName))
                            {
                                ApplyOperator(ref query, propertyName, op, val);
                            }
                            continue;
                        }

                        // Obtener el nombre real de la propiedad raíz
                        var rootProperty = GetActualPropertyName(typeof(T), parts[0]);
                        if (string.IsNullOrEmpty(rootProperty))
                        {
                            Console.WriteLine($"No se encontró la propiedad '{parts[0]}' en la entidad");
                            continue;
                        }

                        // Determinar si la raíz es una colección
                        bool isCollection = IsCollectionProperty(typeof(T), rootProperty);

                        if (isCollection)
                        {
                            // Es una colección (como Modelos)
                            var nestedPath = string.Join(".", parts.Skip(1));

                            if (string.IsNullOrEmpty(nestedPath))
                            {
                                Console.WriteLine($"No se especificó una propiedad para la colección '{rootProperty}'");
                                continue;
                            }

                            // Construir una expresión segura para el filtro en la colección
                            ApplyCollectionOperator(ref query, rootProperty, nestedPath, op, val);
                        }
                        else
                        {
                            // Es una navegación simple (como Equipo)
                            var fullPath = rootProperty;

                            for (int j = 1; j < parts.Length; j++)
                            {
                                var propName = GetActualPropertyName(GetNavigationPropertyType(typeof(T), string.Join(".", parts.Take(j))), parts[j]);
                                if (string.IsNullOrEmpty(propName))
                                {
                                    Console.WriteLine($"No se encontró la propiedad '{parts[j]}' en la ruta '{string.Join(".", parts.Take(j))}'");
                                    fullPath = null;
                                    break;
                                }
                                fullPath += "." + propName;
                            }

                            if (!string.IsNullOrEmpty(fullPath))
                            {
                                ApplyOperator(ref query, fullPath, op, val);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar filtro '{parameters.Filter[i]}': {ex.Message}");
                    }
                }
            }

            // Aplicar ordenación
            if (!string.IsNullOrEmpty(parameters.SortColumn))
            {
                try
                {
                    var column = parameters.SortColumn;
                    var dir = parameters.SortOrder?.ToLower() == "desc" ? "descending" : "ascending";

                    // Dividir la ruta de la propiedad en partes
                    var parts = column.Split('.');

                    // Si solo es una propiedad simple (no hay puntos)
                    if (parts.Length == 1)
                    {
                        var propertyName = GetActualPropertyName(typeof(T), parts[0]);
                        if (!string.IsNullOrEmpty(propertyName))
                        {
                            query = query.OrderBy(_cfg, $"{propertyName} {dir}");
                        }
                    }
                    else
                    {
                        // Obtener el nombre real de la propiedad raíz
                        var rootProperty = GetActualPropertyName(typeof(T), parts[0]);
                        if (string.IsNullOrEmpty(rootProperty))
                        {
                            Console.WriteLine($"No se encontró la propiedad '{parts[0]}' en la entidad para ordenamiento");
                        }
                        else
                        {
                            // Determinar si la raíz es una colección
                            bool isCollection = IsCollectionProperty(typeof(T), rootProperty);

                            if (isCollection)
                            {
                                // Para colecciones, usar Max/Min
                                var nestedPath = string.Join(".", parts.Skip(1));

                                if (string.IsNullOrEmpty(nestedPath))
                                {
                                    Console.WriteLine($"No se especificó una propiedad para la colección '{rootProperty}' en ordenamiento");
                                }
                                else
                                {
                                    // Usar Min para asc, Max para desc
                                    var aggregator = dir == "ascending" ? "Min" : "Max";
                                    query = query.OrderBy(_cfg, $"{rootProperty}.{aggregator}(item => item.{nestedPath}) {dir}");
                                }
                            }
                            else
                            {
                                // Es una navegación simple (como Equipo)
                                var fullPath = rootProperty;
                                bool isValid = true;

                                for (int j = 1; j < parts.Length; j++)
                                {
                                    var propName = GetActualPropertyName(GetNavigationPropertyType(typeof(T), string.Join(".", parts.Take(j))), parts[j]);
                                    if (string.IsNullOrEmpty(propName))
                                    {
                                        Console.WriteLine($"No se encontró la propiedad '{parts[j]}' en la ruta '{string.Join(".", parts.Take(j))}' para ordenamiento");
                                        isValid = false;
                                        break;
                                    }
                                    fullPath += "." + propName;
                                }

                                if (isValid)
                                {
                                    query = query.OrderBy(_cfg, $"{fullPath} {dir}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al aplicar ordenamiento: {ex.Message}");
                }
            }

            // Aplicar los includes con ThenInclude
            query = includeFunc(query);

            //dataCount = await query.CountAsync();

            // Aplicar paginación
            if (parameters.Page.HasValue && parameters.PageSize.HasValue && parameters.Page > 0 && parameters.PageSize > 0)
            {
                query = query.Skip((parameters.Page.Value - 1) * parameters.PageSize.Value).Take(parameters.PageSize.Value);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        // Métodos auxiliares para el manejo de propiedades
        private Type GetPropertyType(Type entityType, string propertyName)
        {
            var property = entityType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return property?.PropertyType;
        }

        private bool IsNumericType(Type type)
        {
            if (type == null) return false;

            // Manejar tipos nullable
            type = Nullable.GetUnderlyingType(type) ?? type;

            return type == typeof(int) || type == typeof(long) || type == typeof(decimal) ||
                   type == typeof(double) || type == typeof(float) || type == typeof(short) ||
                   type == typeof(byte);
        }

        private bool IsStringType(Type type)
        {
            return type == typeof(string);
        }
        private string GetActualPropertyName(Type type, string propertyName)
        {
            if (type == null) return null;

            // Buscar primero en las propiedades
            var property = type.GetProperties()
                .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));

            if (property != null)
                return property.Name;

            // Si no se encuentra, buscar en las navegaciones de EF Core
            var entityType = _context.Model.FindEntityType(type);
            if (entityType != null)
            {
                var navigation = entityType.GetNavigations()
                    .FirstOrDefault(n => string.Equals(n.Name, propertyName, StringComparison.OrdinalIgnoreCase));

                if (navigation != null)
                    return navigation.Name;
            }

            return null;
        }

        private bool IsCollectionProperty(Type type, string propertyName)
        {
            if (type == null || string.IsNullOrEmpty(propertyName)) return false;

            // Buscar la propiedad
            var property = type.GetProperty(propertyName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.IgnoreCase);

            if (property != null)
            {
                // Verificar si es una colección
                return typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) &&
                       property.PropertyType != typeof(string);
            }

            // Verificar en las navegaciones de EF Core
            var entityType = _context.Model.FindEntityType(type);
            if (entityType != null)
            {
                var navigation = entityType.GetNavigations()
                    .FirstOrDefault(n => string.Equals(n.Name, propertyName, StringComparison.OrdinalIgnoreCase));

                if (navigation != null)
                {
                    return typeof(System.Collections.IEnumerable).IsAssignableFrom(navigation.ClrType) &&
                           navigation.ClrType != typeof(string);
                }
            }

            return false;
        }

        private Type GetNavigationPropertyType(Type type, string path)
        {
            if (type == null || string.IsNullOrEmpty(path)) return null;

            var parts = path.Split('.');
            Type currentType = type;

            foreach (var part in parts)
            {
                var property = currentType.GetProperty(part,
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.IgnoreCase);

                if (property == null)
                {
                    // Buscar en las navegaciones de EF Core
                    var entityType = _context.Model.FindEntityType(currentType);
                    if (entityType != null)
                    {
                        var navigation = entityType.GetNavigations()
                            .FirstOrDefault(n => string.Equals(n.Name, part, StringComparison.OrdinalIgnoreCase));

                        if (navigation != null)
                        {
                            // Si es una colección, obtener el tipo de elemento
                            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(navigation.ClrType) &&
                                navigation.ClrType != typeof(string))
                            {
                                if (navigation.ClrType.IsGenericType)
                                {
                                    var elementType = navigation.ClrType.GetGenericArguments().FirstOrDefault();
                                    if (elementType != null)
                                    {
                                        currentType = elementType;
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                currentType = navigation.ClrType;
                                continue;
                            }
                        }
                    }

                    return null;
                }

                // Si es una colección, obtener el tipo de elemento
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) &&
                    property.PropertyType != typeof(string))
                {
                    if (property.PropertyType.IsGenericType)
                    {
                        var elementType = property.PropertyType.GetGenericArguments().FirstOrDefault();
                        if (elementType != null)
                        {
                            currentType = elementType;
                            continue;
                        }
                    }
                    else if (property.PropertyType.IsArray)
                    {
                        currentType = property.PropertyType.GetElementType();
                        continue;
                    }

                    return null;
                }

                currentType = property.PropertyType;
            }

            return currentType;
        }

        private void ApplyOperator(ref IQueryable<T> query, string propertyPath, string op, string value)
        {
            string expr = string.Empty;

            switch (op)
            {
                case "eq":
                    expr = $"{propertyPath} == @0";
                    break;
                case "neq":
                    expr = $"{propertyPath} != @0";
                    break;
                case "gt":
                    expr = $"{propertyPath} > @0";
                    break;
                case "lt":
                    expr = $"{propertyPath} < @0";
                    break;
                case "gte":
                    expr = $"{propertyPath} >= @0";
                    break;
                case "lte":
                    expr = $"{propertyPath} <= @0";
                    break;
                case "like":
                    expr = $"{propertyPath}.ToString().ToLower().Contains(@0.ToLower())";
                    break;
                case "notlike":
                    expr = $"!{propertyPath}.ToString().ToLower().Contains(@0.ToLower())";
                    break;
                case "startswith":
                    expr = $"{propertyPath}.ToString().ToLower().StartsWith(@0.ToLower())";
                    break;
                case "endswith":
                    expr = $"{propertyPath}.ToString().ToLower().EndsWith(@0.ToLower())";
                    break;
                default:
                    Console.WriteLine($"Operador desconocido: {op}");
                    return;
            }

            Console.WriteLine($"Expresión de filtro: {expr}");

            try
            {
                query = query.Where(_cfg, expr, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al aplicar filtro: {ex.Message}");
            }
        }

        private void ApplyCollectionOperator(ref IQueryable<T> query, string collectionPath, string propertyPath, string op, string value)
        {
            string expr = string.Empty;

            switch (op)
            {
                case "eq":
                    expr = $"{collectionPath}.Any(item => item.{propertyPath} == @0)";
                    break;
                case "neq":
                    expr = $"{collectionPath}.Any(item => item.{propertyPath} != @0)";
                    break;
                case "gt":
                    expr = $"{collectionPath}.Any(item => item.{propertyPath} > @0)";
                    break;
                case "lt":
                    expr = $"{collectionPath}.Any(item => item.{propertyPath} < @0)";
                    break;
                case "gte":
                    expr = $"{collectionPath}.Any(item => item.{propertyPath} >= @0)";
                    break;
                case "lte":
                    expr = $"{collectionPath}.Any(item => item.{propertyPath} <= @0)";
                    break;
                case "like":
                    expr = $"{collectionPath}.Any(item => item.{propertyPath}.ToString().ToLower().Contains(@0.ToLower()))";
                    break;
                case "notlike":
                    expr = $"!{collectionPath}.Any(item => item.{propertyPath}.ToString().ToLower().Contains(@0.ToLower()))";
                    break;
                case "startswith":
                    expr = $"{collectionPath}.Any(item => item.{propertyPath}.ToString().ToLower().StartsWith(@0.ToLower()))";
                    break;
                case "endswith":
                    expr = $"{collectionPath}.Any(item => item.{propertyPath}.ToString().ToLower().EndsWith(@0.ToLower()))";
                    break;
                default:
                    Console.WriteLine($"Operador desconocido: {op}");
                    return;
            }

            Console.WriteLine($"Expresión de filtro en colección: {expr}");

            try
            {
                query = query.Where(_cfg, expr, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al aplicar filtro en colección: {ex.Message}");
            }
        }

        // También actualizar CountAsync con la misma lógica
        public async Task<int> CountAsync(DinamicFilters parameters, Func<IQueryable<T>, IQueryable<T>> includeFunc)
        {
            var query = _dbset.AsQueryable().Where(x => x.Activo == true);

            // ---- búsqueda rápida ----
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm) &&
                parameters.SearchColumns?.Any() == true)
            {
                var searchConditions = new List<string>();
                var searchValue = parameters.SearchTerm.ToLower();

                foreach (var column in parameters.SearchColumns)
                {
                    try
                    {
                        // Dividir la ruta de la propiedad en partes
                        var parts = column.Split('.');

                        // Si solo es una propiedad simple (no hay puntos)
                        if (parts.Length == 1)
                        {
                            var propertyName = GetActualPropertyName(typeof(T), parts[0]);
                            if (!string.IsNullOrEmpty(propertyName))
                            {
                                // Usar EF.Functions.Like en lugar de ToString().Contains para propiedades string
                                searchConditions.Add($"{propertyName}.Contains(@0)");
                            }
                            continue;
                        }

                        // Obtener el nombre real de la propiedad raíz
                        var rootProperty = GetActualPropertyName(typeof(T), parts[0]);
                        if (string.IsNullOrEmpty(rootProperty))
                        {
                            Console.WriteLine($"No se encontró la propiedad '{parts[0]}' en la entidad");
                            continue;
                        }

                        // Determinar si la raíz es una colección
                        bool isCollection = IsCollectionProperty(typeof(T), rootProperty);

                        if (isCollection)
                        {
                            // Es una colección (como Modelos)
                            var nestedPath = string.Join(".", parts.Skip(1));

                            if (string.IsNullOrEmpty(nestedPath))
                            {
                                Console.WriteLine($"No se especificó una propiedad para la colección '{rootProperty}'");
                                continue;
                            }

                            // Construir una expresión segura para la búsqueda en la colección (sin ToString)
                            searchConditions.Add($"{rootProperty}.Any(item => item.{nestedPath}.Contains(@0))");
                        }
                        else
                        {
                            // Es una navegación simple (como Equipo)
                            var fullPath = rootProperty;

                            for (int i = 1; i < parts.Length; i++)
                            {
                                var propName = GetActualPropertyName(GetNavigationPropertyType(typeof(T), string.Join(".", parts.Take(i))), parts[i]);
                                if (string.IsNullOrEmpty(propName))
                                {
                                    Console.WriteLine($"No se encontró la propiedad '{parts[i]}' en la ruta '{string.Join(".", parts.Take(i))}'");
                                    fullPath = null;
                                    break;
                                }
                                fullPath += "." + propName;
                            }

                            if (!string.IsNullOrEmpty(fullPath))
                            {
                                // Usar EF.Functions.Like en lugar de ToString().Contains
                                searchConditions.Add($"{fullPath}.Contains(@0)");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar columna de búsqueda '{column}': {ex.Message}");
                    }
                }

                if (searchConditions.Any())
                {
                    var searchExpression = string.Join(" OR ", searchConditions);
                    Console.WriteLine($"Expresión de búsqueda (count): {searchExpression}");

                    try
                    {
                        // Modificar el parámetro de búsqueda para usar Like
                        query = query.Where(_cfg, searchExpression, searchValue);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al aplicar búsqueda (count): {ex.Message}");
                    }
                }
            }

            // Aplicar filtros dinámicos
            if (parameters.Filter != null && parameters.FilterOperator != null && parameters.FilterValue != null &&
                parameters.Filter.Count > 0 && parameters.Filter.Count == parameters.FilterOperator.Count &&
                parameters.Filter.Count == parameters.FilterValue.Count)
            {
                for (int i = 0; i < parameters.Filter.Count; i++)
                {
                    try
                    {
                        var column = parameters.Filter[i];
                        var op = parameters.FilterOperator[i].ToLower();
                        var val = parameters.FilterValue[i];

                        // Dividir la ruta de la propiedad en partes
                        var parts = column.Split('.');

                        // Si solo es una propiedad simple (no hay puntos)
                        if (parts.Length == 1)
                        {
                            var propertyName = GetActualPropertyName(typeof(T), parts[0]);
                            if (!string.IsNullOrEmpty(propertyName))
                            {
                                ApplyOperator(ref query, propertyName, op, val);
                            }
                            continue;
                        }

                        // Obtener el nombre real de la propiedad raíz
                        var rootProperty = GetActualPropertyName(typeof(T), parts[0]);
                        if (string.IsNullOrEmpty(rootProperty))
                        {
                            Console.WriteLine($"No se encontró la propiedad '{parts[0]}' en la entidad");
                            continue;
                        }

                        // Determinar si la raíz es una colección
                        bool isCollection = IsCollectionProperty(typeof(T), rootProperty);

                        if (isCollection)
                        {
                            // Es una colección (como Modelos)
                            var nestedPath = string.Join(".", parts.Skip(1));

                            if (string.IsNullOrEmpty(nestedPath))
                            {
                                Console.WriteLine($"No se especificó una propiedad para la colección '{rootProperty}'");
                                continue;
                            }

                            // Construir una expresión segura para el filtro en la colección
                            ApplyCollectionOperator(ref query, rootProperty, nestedPath, op, val);
                        }
                        else
                        {
                            // Es una navegación simple (como Equipo)
                            var fullPath = rootProperty;

                            for (int j = 1; j < parts.Length; j++)
                            {
                                var propName = GetActualPropertyName(GetNavigationPropertyType(typeof(T), string.Join(".", parts.Take(j))), parts[j]);
                                if (string.IsNullOrEmpty(propName))
                                {
                                    Console.WriteLine($"No se encontró la propiedad '{parts[j]}' en la ruta '{string.Join(".", parts.Take(j))}'");
                                    fullPath = null;
                                    break;
                                }
                                fullPath += "." + propName;
                            }

                            if (!string.IsNullOrEmpty(fullPath))
                            {
                                ApplyOperator(ref query, fullPath, op, val);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar filtro '{parameters.Filter[i]}': {ex.Message}");
                    }
                }
            }

            // Aplicar los includes con ThenInclude
            query = includeFunc(query);

            return await query.AsNoTracking().CountAsync();
        }

        // Nuevo método con soporte para ThenInclude
        public async Task<T?> GetByIdAsync(long id, Func<IQueryable<T>, IQueryable<T>> includeFunc)
        {
            // Obtener todas las propiedades de clave primaria de la entidad
            var keyNames = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties
                .Select(p => p.Name).ToList();

            if (keyNames == null || keyNames.Count == 0)
            {
                // Si no podemos determinar las claves, retornamos null
                return null;
            }

            // Creamos un nuevo query con los includes usando la función ThenInclude
            var query = _dbset.AsQueryable();

            // Aplicamos la función de inclusión que contiene ThenInclude
            query = includeFunc(query);

            // Construimos un predicado dinámico para buscar por la clave primaria
            var parameter = Expression.Parameter(typeof(T), "e");
            var keyProperty = Expression.Property(parameter, keyNames.First());
            var keyValue = Expression.Constant(id);
            var equalExpr = Expression.Equal(keyProperty, keyValue);
            var lambda = Expression.Lambda<Func<T, bool>>(equalExpr, parameter);

            // Ejecutamos la consulta
            return await query.FirstOrDefaultAsync(lambda);
        }

        // Método para normalizar las rutas de propiedades
        private string[] NormalizePropertyPaths(List<string> propertyPaths)
        {
            return propertyPaths.Select(p => NormalizePropertyPath(p, true)).ToArray();
        }

        // Detecta si una ruta de propiedad raíz es una colección de navegación
        private bool IsCollectionNavigation(string propertyPath)
        {
            var root = propertyPath.Split('.')[0];
            var entityType = _context.Model.FindEntityType(typeof(T));
            var nav = entityType?.FindNavigation(root);
            return nav != null
                   && typeof(System.Collections.IEnumerable).IsAssignableFrom(nav.ClrType)
                   && nav.ClrType != typeof(string);
        }

        // Construye la expresión de orden dinámico
        private string BuildOrderExpression(string sortColumn, string sortOrder)
        {
            var order = sortOrder?.ToUpper() == "DESC" ? "descending" : "ascending";
            if (IsCollectionNavigation(sortColumn))
            {
                var parts = sortColumn.Split('.');
                var coll = parts[0];
                var nested = string.Join(".", parts.Skip(1));
                // Por convención, Min para ASC, Max para DESC
                var agg = order == "descending" ? "Max" : "Min";
                return $"{coll}.{agg}({nested}) {order}";
            }

            var norm = NormalizePropertyPath(sortColumn, false);
            return $"{norm} {order}";
        }


        // Construye la expresión de filtro dinámica y devuelve el filtro (nullable) + valor (nullable)
        private (string? filter, object? value) BuildFilterExpression(string propertyPath, string operatorCode, string valueStr, bool forSearch)
        {
            // 1) Colección -> tratar anidado
            if (IsCollectionNavigation(propertyPath))
            {
                var parts = propertyPath.Split('.');
                var coll = parts[0];
                var nested = string.Join(".", parts.Skip(1));
                var p = "item";

                // Intentar detectar tipo del nested (limitado: sólo último segmento)
                Type? nestedType = null;
                try
                {
                    // Caminamos para intentar obtener el tipo final
                    var currentType = typeof(T);
                    foreach (var seg in parts.Skip(1))
                    {
                        var pi = currentType.GetProperty(seg,
                            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (pi == null) { nestedType = null; break; }
                        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(pi.PropertyType) && pi.PropertyType != typeof(string))
                        {
                            // Colección intermedia
                            if (pi.PropertyType.IsGenericType)
                                currentType = pi.PropertyType.GetGenericArguments()[0];
                            else
                                break;
                        }
                        else
                        {
                            currentType = pi.PropertyType;
                        }
                        nestedType = currentType;
                    }
                }
                catch { /* ignorar */ }

                string op = operatorCode.ToLower();
                bool isTextual = op is "like" or "notlike" or "startswith" or "endswith";
                bool isString = nestedType == typeof(string);
                bool isNumeric = nestedType != null && IsNumericType(nestedType);

                // Reglas para operadores textuales sobre tipos no string
                if (isTextual && !isString)
                {
                    // Si es búsqueda rápida y el valor es numérico y la columna numérica -> degradar a igualdad
                    if (isNumeric && decimal.TryParse(valueStr, out _))
                    {
                        // Solo aplicamos para "like": el resto (startswith/endswith) se ignoran
                        if (op == "like")
                        {
                            return ($"{coll}.Any({p} => {p}.{nested} == @0)", Convert.ChangeType(valueStr, Nullable.GetUnderlyingType(nestedType!) ?? nestedType!));
                        }
                        // No tiene sentido startswith/endswith sobre numerics -> ignorar
                        return (null, null);
                    }
                    // Ignorar columnas no string para operadores textuales
                    return (null, null);
                }

                string? expr = op switch
                {
                    "eq" => $"{coll}.Any({p} => {p}.{nested} == @0)",
                    "neq" => $"{coll}.Any({p} => {p}.{nested} != @0)",
                    "gt" => $"{coll}.Any({p} => {p}.{nested} > @0)",
                    "lt" => $"{coll}.Any({p} => {p}.{nested} < @0)",
                    "gte" => $"{coll}.Any({p} => {p}.{nested} >= @0)",
                    "lte" => $"{coll}.Any({p} => {p}.{nested} <= @0)",
                    "like" => $"{coll}.Any({p} => {p}.{nested}.Contains(@0))",
                    "notlike" => $"!{coll}.Any({p} => {p}.{nested}.Contains(@0))",
                    "startswith" => $"{coll}.Any({p} => {p}.{nested}.StartsWith(@0))",
                    "endswith" => $"{coll}.Any({p} => {p}.{nested}.EndsWith(@0))",
                    _ => null
                };

                object finalVal = valueStr;
                if (nestedType != null && op is not ("like" or "notlike" or "startswith" or "endswith"))
                {
                    try
                    {
                        finalVal = Convert.ChangeType(valueStr, Nullable.GetUnderlyingType(nestedType) ?? nestedType);
                    }
                    catch { }
                }

                return expr != null ? (expr, finalVal) : (null, null);
            }

            // 2) Propiedad simple
            string normalized = NormalizePropertyPath(propertyPath, forSearch);
            PropertyInfo? propInfo = typeof(T).GetProperty(normalized,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            Type? propType = propInfo != null
                ? Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType
                : typeof(string);

            string opLower = operatorCode.ToLower();
            bool textualOp = opLower is "like" or "notlike" or "startswith" or "endswith";
            bool isStringProp = propType == typeof(string);
            bool isNumericProp = IsNumericType(propType);

            // Reglas para operadores textuales sobre propiedades no string
            if (textualOp && !isStringProp)
            {
                if (isNumericProp && opLower == "like" && decimal.TryParse(valueStr, out _))
                {
                    // Degradar a igualdad
                    object numVal;
                    try { numVal = Convert.ChangeType(valueStr, propType); }
                    catch { numVal = valueStr; }
                    return ($"{normalized} == @0", numVal);
                }
                return (null, null); // Ignorar
            }

            object converted;
            try
            {
                converted = Convert.ChangeType(valueStr ?? string.Empty, propType);
            }
            catch
            {
                converted = valueStr;
            }

            string? opExpr = opLower switch
            {
                "eq" => $"{normalized} == @0",
                "neq" => $"{normalized} != @0",
                "gt" => $"{normalized} > @0",
                "lt" => $"{normalized} < @0",
                "gte" => $"{normalized} >= @0",
                "lte" => $"{normalized} <= @0",
                "like" => $"{normalized}.Contains(@0)",
                "notlike" => $"!{normalized}.Contains(@0)",
                "startswith" => $"{normalized}.StartsWith(@0)",
                "endswith" => $"{normalized}.EndsWith(@0)",
                _ => null
            };

            return opExpr != null ? (opExpr, converted) : (null, null);
        }

        // ---------- Helpers para filtros dinámicos ----------
        private static string? BuildScalarExpr(string col, string op) => op switch
        {
            "eq" => $"{col} == @0",
            "neq" => $"{col} != @0",
            "gt" => $"{col} >  @0",
            "lt" => $"{col} <  @0",
            "gte" => $"{col} >= @0",
            "lte" => $"{col} <= @0",
            "like" => $"{col}.ToLower().Contains(@0.ToLower())",
            "notlike" => $"!{col}.ToLower().Contains(@0.ToLower())",
            "startswith" => $"{col}.ToLower().StartsWith(@0.ToLower())",
            "endswith" => $"{col}.ToLower().EndsWith(@0.ToLower())",
            _ => null
        };

        private static string? BuildAnyExpr(string root, string rest, string op) => op switch
        {
            "eq" => $"{root}.Any(it => it.{rest} == @0)",
            "neq" => $"{root}.Any(it => it.{rest} != @0)",
            "gt" => $"{root}.Any(it => it.{rest} >  @0)",
            "lt" => $"{root}.Any(it => it.{rest} <  @0)",
            "gte" => $"{root}.Any(it => it.{rest} >= @0)",
            "lte" => $"{root}.Any(it => it.{rest} <= @0)",
            "like" => $"{root}.Any(it => it.{rest}.ToLower().Contains(@0.ToLower()))",
            "notlike" => $"!{root}.Any(it => it.{rest}.ToLower().Contains(@0.ToLower()))",
            "startswith" => $"{root}.Any(it => it.{rest}.ToLower().StartsWith(@0.ToLower()))",
            "endswith" => $"{root}.Any(it => it.{rest}.ToLower().EndsWith(@0.ToLower()))",
            _ => null
        };

        private bool RootIsCollection(string root)
        {
            var nav = _context.Model.FindEntityType(typeof(T))?.FindNavigation(root);
            return nav != null &&
                    typeof(System.Collections.IEnumerable).IsAssignableFrom(nav.ClrType)
                    && nav.ClrType != typeof(string);
        }

    }
}