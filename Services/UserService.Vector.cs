using Tokengram.Database.Tokengram.Entities;
using Tokengram.Services.Interfaces;
using Tokengram.Constants;
using Tokengram.Database.Neo4j.Nodes;
using Tokengram.Utils;

namespace Tokengram.Services
{
    public partial class UserService : IUserService
    {
        public async Task<User> UpdateUserVectorByAction(User user, Post post, int actionCoefficient)
        {
            var nft = await _nftService.GetNFT(post.NFTAddress);
            var nftVector = nft?.NFT?.NFTVector;
            if (nftVector == null)
                return user;

            user.UserVector = GetNewVector(user.UserVector, nftVector, actionCoefficient);

            await _tokengramDbContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUserVectorByLike(User user, Post post)
        {
            return await UpdateUserVectorByAction(user, post, UpdatingVectorCoefficients.LIKE_COEFFICIENT);
        }

        public async Task<User> UpdateUserVectorByUnlike(User user, Post post)
        {
            return await UpdateUserVectorByAction(user, post, -1 * UpdatingVectorCoefficients.LIKE_COEFFICIENT);
        }

        public async Task<User> UpdateUserVectorByComment(User user, Post post)
        {
            return await UpdateUserVectorByAction(user, post, UpdatingVectorCoefficients.COMMENT_COEFFICIENT);
        }

        public async Task<User> UpdateUserVectorByUncomment(User user, Post post)
        {
            return await UpdateUserVectorByAction(user, post, -1 * UpdatingVectorCoefficients.COMMENT_COEFFICIENT);
        }

        public async Task<User> UpdateUserVectorByNewNFT(User user, NFTNode nft)
        {
            user.UserVector = GetNewVector(
                user.UserVector,
                nft?.NFTVector ?? string.Empty,
                UpdatingVectorCoefficients.NEW_NFT_COEFFICIENT
            );

            await _tokengramDbContext.SaveChangesAsync();
            return user;
        }

        public string SerializeVector(Dictionary<string, int> vector)
        {
            return string.Join(";", vector.Select(kv => $"{kv.Key}:{kv.Value}"));
        }

        public string GetNewVector(string userVector, string nftVector, int actionCoefficient)
        {
            var updatedVector = CosineSimilarityUtil.ParseVectorString(userVector);

            var totalSum = updatedVector.Values.Sum(); // Total sum of values in the vector
            var maxVectorSum = 100; // Maximum sum of the vector
            var maxWeight = maxVectorSum / Math.Max(updatedVector.Count, 0); // Maximum weight for each tag

            // Update vector based on nftVector and actionCoefficient
            foreach (var tagWeight in CosineSimilarityUtil.ParseVectorString(nftVector))
            {
                var tag = tagWeight.Key;
                var weight = tagWeight.Value;

                if (!updatedVector.ContainsKey(tag))
                {
                    updatedVector[tag] = Math.Min(weight * actionCoefficient, maxWeight);
                }
                else
                {
                    updatedVector[tag] += Math.Min(weight * actionCoefficient, maxWeight);
                }
            }

            // Normalize the vector based on the maximum sum
            var currentSum = Math.Max(updatedVector.Values.Sum(), 1);
            var factor = currentSum > maxVectorSum ? maxVectorSum / (double)currentSum : 1;

            var keysToDelete = updatedVector.Where(kv => kv.Value < 0.50).Select(kv => kv.Key).ToList();

            foreach (var tag in keysToDelete)
            {
                updatedVector.Remove(tag);
            }

            foreach (var tag in updatedVector.Keys.ToList())
            {
                updatedVector[tag] = (int)(updatedVector[tag] * factor);
            }

            var UserVector = SerializeVector(updatedVector);

            return UserVector;
        }
    }
}
