using ConsoleApp1.Models;

namespace ConsoleApp1.Interfaces
{
    public interface IPromotion
    {
        Task<IEnumerable<Promotion>> GetAllPromotionsAsync();
        Task<Promotion> GetPromotionAsync(int id);

        Task AddPromotionAsync(Promotion promotion);
        Task EditPromotionAsync(Promotion promotion);
        Task DeletePromotionAsync(Promotion promotion);
    }
}
