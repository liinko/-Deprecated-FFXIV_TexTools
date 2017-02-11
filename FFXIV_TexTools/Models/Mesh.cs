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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIV_TexTools.Models
{
    public class Mesh
    {
        public int numVerts, numIndex, materialNumber, partTableOffset, partTableCount, boneListIndex, indexDataOffset, numBuffers;
        public int[] vertexDataOffsets = new int[3];
        public int[] vertexSizes = new int[3];
        public MeshData[] meshData;
        public byte[] indexData;
        public Mesh()
        {

        }

        public void setMeshData()
        {
            meshData = new MeshData[numBuffers];
        }
    }
}
