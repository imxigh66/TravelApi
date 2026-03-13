using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips
{
    public class TripNoteRequest
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
