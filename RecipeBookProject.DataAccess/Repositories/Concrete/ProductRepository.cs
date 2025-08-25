using RecipeBookProject.Data.Context;
using RecipeBookProject.Data.Entities;
using RecipeBookProject.DataAccess.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.DataAccess.Repositories.Concrete;
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(RecipeBookProjectDbContext db) : base(db) { }
}