using System;
using AutoMapper;
using DearHome_Backend.DTOs;
using DearHome_Backend.DTOs.AddressDtos;
using DearHome_Backend.DTOs.AttributeDtos;
using DearHome_Backend.DTOs.AttributeValueDtos;
using DearHome_Backend.DTOs.CategoryAttributeDtos;
using DearHome_Backend.DTOs.CategoryDtos;
using DearHome_Backend.DTOs.GoodReceivedItemDtos;
using DearHome_Backend.DTOs.GoodReceivedNoteDtos;
using DearHome_Backend.DTOs.PlacementDtos;
using DearHome_Backend.DTOs.ProductDtos;
using DearHome_Backend.DTOs.UserDtos;
using DearHome_Backend.DTOs.VariantAttributeDtos;
using DearHome_Backend.DTOs.VariantDtos;
using DearHome_Backend.Modals;
using DearHome_Backend.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DearHome_Backend.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserRegisterDto, User>();
        CreateMap<UserLoginDto, User>();
        CreateMap<UpdateUserDto, User>();

        CreateMap<UpdateAddressDto, Address>();

        CreateMap<AddCategoryDto, Category>();
        CreateMap<Category, CategoryDto>();
        CreateMap<UpdateCategoryDto, Category>();

        CreateMap<AddAttributeDto, Models.Attribute>();
        CreateMap<Models.Attribute, AttributeDto>();
        CreateMap<UpdateAttributeDto, Models.Attribute>();

        CreateMap<AddCategoryAttributeDto, CategoryAttribute>();
        CreateMap<CategoryAttribute, CategoryAttributeDto>();
        CreateMap<UpdateCategoryAttributeDto, CategoryAttribute>();

        CreateMap<UpdatePlacementDto, Placement>(); 
        CreateMap<Placement, PlacementDto>();
        CreateMap<AddPlacementDto, Placement>();

        CreateMap<AddAttributeValueDto, AttributeValue>();
        CreateMap<AttributeValue, AttributeValueDto>();
        CreateMap<UpdateAttributeValueDto, AttributeValue>();

        CreateMap<AddProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        CreateMap<AddVariantDto, Variant>();
        CreateMap<UpdateVariantDto, Variant>();
        CreateMap<Variant, VariantDto>();

        CreateMap<AddVariantAttributeDto, VariantAttribute>();
        CreateMap<VariantAttribute, VariantAttributeDto>();
        CreateMap<UpdateVariantAttributeDto, VariantAttribute>();

        CreateMap<AddGoodReceivedNoteDto, GoodReceivedNote>();
        CreateMap<GoodReceivedNote, GoodReceivedNoteDto>();
        CreateMap<UpdateGoodReceivedNoteDto, GoodReceivedNote>();

        CreateMap<AddGoodReceivedItemDto, GoodReceivedItem>();
        CreateMap<GoodReceivedItem, GoodReceivedItemDto>();
        CreateMap<UpdateGoodReceivedItemDto, GoodReceivedItem>();
    }
}
