using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public class MaterialDetail
    {
        private Dictionary<TrashMaterialType, int> _materialDetails;

        public Dictionary<TrashMaterialType, int> MaterialDetails
        {
            get => _materialDetails;
            set => _materialDetails = value;
        }

        public MaterialDetail(List<TrashMaterialData> materials)
        {
            _materialDetails = new Dictionary<TrashMaterialType, int>
            {
                { TrashMaterialType.Glass, materials.Count(mat => mat.type == TrashMaterialType.Glass) },
                { TrashMaterialType.Metal, materials.Count(mat => mat.type == TrashMaterialType.Metal) },
                { TrashMaterialType.Paper, materials.Count(mat => mat.type == TrashMaterialType.Paper) },
                { TrashMaterialType.Plastic, materials.Count(mat => mat.type == TrashMaterialType.Plastic) },
                { TrashMaterialType.Wood, materials.Count(mat => mat.type == TrashMaterialType.Wood) }
            };
        }
        
        public override bool Equals(object obj)
        {
            var item = obj as MaterialDetail;

            if (item == null)
            {
                return false;
            }

            return CompareMaterialDetails(this, item);
        }

        private bool CompareMaterialDetails(MaterialDetail mat1, MaterialDetail mat2)
        {
            foreach (KeyValuePair<TrashMaterialType, int> detail in mat1.MaterialDetails)
            {
                if (mat1.MaterialDetails[detail.Key] != mat2.MaterialDetails[detail.Key])
                {
                    return false;
                }
            }
            return true;
        }
    }
}