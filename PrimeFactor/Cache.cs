// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

using System;
using System.Collections.Generic;

namespace PrimeFactor;

public class Cache
{
	private const int ChunkSizeInBytes = 64 * 1024 * 1024; // 64MB=2^26
	private const int ElementsPerChunk = ChunkSizeInBytes / sizeof(ulong); // 8,388,608 = 2^23

	private readonly List<ulong[]> _chunks;

	public Cache()
	{
		_chunks = new List<ulong[]>();
	}

	/// <summary>
	/// Gets the total number of elements stored.
	/// </summary>
	public long TotalElements => (long)_chunks.Count * ElementsPerChunk;

	/// <summary>
	/// Adds a new 64MB chunk to the cache.
	/// </summary>
	public void AddChunk()
	{
		_chunks.Add(new ulong[ElementsPerChunk]);
	}

	/// <summary>
	/// Gets or sets a value at the global index.
	/// </summary>
	public ulong this[long index]
	{
		get
		{
			var (chunkIndex, offset) = GetChunkAndOffset(index);
			return _chunks[chunkIndex][offset];
		}

		set
		{
			var (chunkIndex, offset) = GetChunkAndOffset(index);
			_chunks[chunkIndex][offset] = value;
		}
	}


	/// Returns number of chunks currently in cache.
	public int ChunkCount => _chunks.Count;

	private (int chunkIndex, int offset) GetChunkAndOffset(long globalIndex)
	{
		if (globalIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(globalIndex));

		int chunkIndex = (int)(globalIndex / ElementsPerChunk);
		int offset = (int)(globalIndex % ElementsPerChunk);

		if (chunkIndex >= _chunks.Count)
			throw new IndexOutOfRangeException("Index exceeds cached data.");

		return (chunkIndex, offset);
	}
}
