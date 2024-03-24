using System;

namespace Tokengram.Services.Interfaces
{
    public interface ICosineSimilarityService
    {
        double GetCosineSimilarity(double[] userVector, double[] nftVector);
    }
}
