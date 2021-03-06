﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonInterfaces.Inventory
{
    public interface IInventoryItem
    {
        string Id { get; }
        string Name { get; set; }
        IItemOwner Owner { get; set; }
    }
}
