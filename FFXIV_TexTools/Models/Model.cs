// FFXIV TexTools
// Copyright © 2017 Rafael Gonzalez - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.


namespace FFXIV_TexTools.Models
{
    class Model
    {
        public ModelQuality[] Quality = new ModelQuality[3];
        public int numStrings, stringBlockSize, numTotalMeshes, numAtrStrings, numParts, numMaterialStrings, numBoneStrings, numBoneLists, boneIndexSize;
        public int unk1, unk2, unk3, unk4, unk5, unk6, unk7;
        public MeshPart[] meshPart = null;
        public BoneList[] boneList;
        public int[] boneIndicies;
        public BoundingBoxes[] bb = new BoundingBoxes[4];


        /// <summary>
        /// Sets up 3D model data
        /// </summary>
        /// <param name="numTotalMeshes">The number of meshes the model contains</param>
        /// <param name="MaterialCount">The number of materials the model contains</param>
        public Model(int numTotalMeshes, int MaterialCount)
        {
            this.numTotalMeshes= numTotalMeshes;
            for(int i = 0; i < 3; i++)
            {
                Quality[i] = new ModelQuality(numTotalMeshes/2);
            }
        }

        public void setMeshParts()
        {
            meshPart = new MeshPart[numParts];
        }

        public void setBoneList()
        {
            boneList = new BoneList[numBoneLists];
        }

        public void setBoneIndicies()
        {
            boneIndicies = new int[boneIndexSize/2];
        }
    }
}
