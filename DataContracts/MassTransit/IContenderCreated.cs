﻿using DataContracts.Dtos;

namespace DataContracts.MassTransit;

public interface IContenderCreated
{
    public ContenderFullNameDto? Name { get; set; }
}