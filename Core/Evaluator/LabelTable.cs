using System.Collections.Generic;
using PixelWallE.Core.Evaluator.Runtime;

namespace PixelWallE.Core.Evaluator
{
    public class LabelTable
    {
        private readonly Dictionary<string, int> labelToIndex = new();

        public void Define(string labelName, int statementIndex)
        {
            if (labelToIndex.ContainsKey(labelName))
            {
                throw new RuntimeError(null, $"Etiqueta duplicada: {labelName}");
            }

            labelToIndex[labelName] = statementIndex;
        }

        public int Resolve(string labelName)
        {
            if (!labelToIndex.TryGetValue(labelName, out var index))
            {
                throw new RuntimeError(null, $"Etiqueta no encontrada: {labelName}");
            }
            return index;
        }

        public bool Contains(string labelName) => labelToIndex.ContainsKey(labelName);
    }
}