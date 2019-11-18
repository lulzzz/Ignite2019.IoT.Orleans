﻿using System.Threading.Tasks;
using Ignite2019.IoT.Orleans.DataAccess;
using Ignite2019.IoT.Orleans.Model;
using Orleans;
using System.Linq;
using WalkingTec.Mvvm.Core;

namespace Ignite2019.IoT.Orleans.Grains.State
{
    public interface ISegmentGrain : IGrainWithGuidKey
    {
        Task<Segment> GetAvailableSegmentAsync(int productId);
    }

    public class SegmentGrain : Grain, ISegmentGrain
    {
        public DataContext DataContext { get; set; }
        public SegmentGrain()
        {
            //this.DataContext = context;
            this.DataContext = new DataContext("Server=(localdb)\\mssqllocaldb;Database=Orleans_db;Trusted_Connection=True;MultipleActiveResultSets=true", DBTypeEnum.SqlServer);
        }
        public async Task<Segment> GetAvailableSegmentAsync(int productId)
        {
            ulong maxSegment = 0;
            var hasSegments = this.DataContext.Set<Segment>().Any();

            if (!hasSegments)
            {
                maxSegment = 0x6400000000;
            }
            else
            {
                var hasProductSegment = this.DataContext.Set<Segment>().Any(sg => sg.ProductId == (int)productId);
                if (hasProductSegment)
                {
                    var availableSegment = this.DataContext.Set<Segment>().FirstOrDefault(sg => sg.ProductId == productId && sg.Remain > 0);
                    if (availableSegment != null)
                    {
                        return availableSegment;
                    }
                }

                maxSegment = this.DataContext.Set<Segment>().Max(s => s.MaxNum);
            }

            var newSegment =
                await this.DataContext.Set<Segment>().AddAsync(Segment.AddNewSegment((int)productId, maxSegment));
            await this.DataContext.SaveChangesAsync();

            return newSegment.Entity;
        }
    }
}