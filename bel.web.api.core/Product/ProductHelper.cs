// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductHelper.cs" company="BEL USA">
//   This product is property of BEL USA.
// </copyright>
// <summary>
//   Defines the ProductHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.Product
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using bel.web.api.core.objects.ImageEffects;
    using bel.web.api.core.objects.Interfaces;
    using bel.web.api.core.objects.Product;

    public class ProductHelper : IProductHelper
    {
        /// <summary>The source path.</summary>
        private readonly string sourcePath;

        /// <summary>The product colors mapping.</summary>
        private readonly Dictionary<ProductDetails, List<EffectParameter>> productColorsMapping;

        /// <summary>The product texture mapping.</summary>
        private readonly Dictionary<ProductDetails, List<EffectParameter>> productTextureMapping;

        /// <summary>Initializes a new instance of the <see cref="ProductHelper"/> class.</summary>
        public ProductHelper()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ProductHelper"/> class.</summary>
        /// <param name="sourcePath">TODO The source path.</param>
        public ProductHelper(string sourcePath)
        {
            this.sourcePath = sourcePath;
            productColorsMapping =
            new Dictionary<ProductDetails, List<EffectParameter>>
                {
                    {
                        new ProductDetails { ProductCode = "PF11", ProductColor = "Brown" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Value = Color.FromArgb(150, 83, 53, 50), Action = EffectAction.Color
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "PF46", ProductColor = "Brown" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Value = Color.FromArgb(150, 118, 51, 35), Action = EffectAction.Color
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "PF55", ProductColor = "Brown" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Value = Color.FromArgb(150, 159, 53, 33), Action = EffectAction.Color
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "PF57", ProductColor = "Brown" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Value = Color.FromArgb(150, 204, 100, 71), Action = EffectAction.Color
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "PF79", ProductColor = "Brown" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Value = Color.FromArgb(150, 100, 54, 41), Action = EffectAction.Color
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "PF62", ProductColor = "*" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Value = Color.FromArgb(150, 24, 24, 24), Action = EffectAction.Color
                                    },
                                new EffectParameter
                                    {
                                        Value = 3, Action = EffectAction.Border
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "MPL02", ProductColor = "Black" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Value = Color.FromArgb(150, 70, 76, 72), Action = EffectAction.Color
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "NOT28", ProductColor = "Black" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Value = $"{this.sourcePath}/lab-assets/1203/Effects/Debossing/NOT28.jpg", Action = EffectAction.Texture
                                    },
                                new EffectParameter
                                    {
                                        Value = 1, Action = EffectAction.Border
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "NOT56", ProductColor = "*" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Value = Color.FromArgb(150, 24, 24, 24), Action = EffectAction.Color
                                    },
                                new EffectParameter
                                    {
                                        Value = 3, Action = EffectAction.Border
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "*", ProductColor = "*" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Value = Color.FromArgb(150, 24, 24, 24), Action = EffectAction.Color
                                    },
                                new EffectParameter
                                    {
                                        Value = 3, Action = EffectAction.Border
                                    }
                            }
                    }
                };
            productTextureMapping = new Dictionary<ProductDetails, List<EffectParameter>>
                {
                    {
                        new ProductDetails
                            {
                                ProductCode = "*", ProductColor = "Silver", ProductMaterial = "Steel"
                            },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Action = EffectAction.Texture,
                                        Value = $"{this.sourcePath}/lab-assets/1203/Effects/Laser/bronze-metallic-2.png"
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "*", ProductColor = "*", ProductMaterial = "Steel" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Action = EffectAction.Texture,
                                        Value = $"{this.sourcePath}/lab-assets/1203/Effects/Laser/silver-metallic-2.jpg"
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "*", ProductColor = "*", ProductMaterial = "Glass" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Action = EffectAction.Texture,
                                        Value = $"{this.sourcePath}/lab-assets/1203/Effects/Laser/frosted-glass6.jpg"
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "*", ProductColor = "*", ProductMaterial = "Ceramic" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Action = EffectAction.Texture,
                                        Value = $"{this.sourcePath}/lab-assets/1203/Effects/Laser/laser ceramic 3.jpg"
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "AB101", ProductColor = "Silver" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Action = EffectAction.Texture,
                                        Value = $"{this.sourcePath}/lab-assets/1203/Effects/Laser/bronze-metallic-2.png"
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "AB101", ProductColor = "*", ProductMaterial = "*" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Action = EffectAction.Texture,
                                        Value = $"{this.sourcePath}/lab-assets/1203/Effects/Laser/silver-metallic-2.jpg"
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "*", ProductColor = "*", ProductMaterial = "*" },
                        new List<EffectParameter>
                        {
                            new EffectParameter
                            {
                                Action = EffectAction.Texture,
                                Value = $"{this.sourcePath}/lab-assets/1203/Effects/Laser/silver-metallic-2.jpg"
                            }
                        }
                    },
                    {
                        new ProductDetails { ProductCode = "TM309", ProductColor = "Silver" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Action = EffectAction.Texture,
                                        Value = @"lab-assets/1203/Effects/Laser/bronze-metallic-2.png"
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "TM343", ProductColor = "Silver" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Action = EffectAction.Texture,
                                        Value = $"{this.sourcePath}/lab-assets/1203/Effects/Laser/bronze-metallic-2.png"
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "TM343", ProductColor = "*" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Action = EffectAction.Texture,
                                        Value = $"{this.sourcePath}/lab-assets/1203/Effects/Laser/silver-metallic-2.jpg"
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "VF34", ProductColor = "*" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Action = EffectAction.Texture,
                                        Value = $"{this.sourcePath}/lab-assets/1203/Effects/Laser/silver-metallic-2.jpg"
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "KEY166", ProductColor = "*" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Action = EffectAction.Texture,
                                        Value = $"{this.sourcePath}/lab-assets/1203/Effects/Laser/silver-metallic-2.jpg"
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "DMAW24", ProductColor = "*" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Action = EffectAction.Texture,
                                        Value = $"{this.sourcePath}/lab-assets/1203/Effects/Laser/frosted-glass6.jpg"
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "DMAW50", ProductColor = "*" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Action = EffectAction.Texture,
                                        Value = $"{this.sourcePath}/lab-assets/1203/Effects/Laser/frosted-glass6.jpg"
                                    }
                            }
                    },
                    {
                        new ProductDetails { ProductCode = "7102", ProductColor = "*" },
                        new List<EffectParameter>
                            {
                                new EffectParameter
                                    {
                                        Action = EffectAction.Texture,
                                        Value = $"{this.sourcePath}/lab-assets/1203/Effects/Laser/laser ceramic 3.jpg"
                                    }
                            }
                    },
                };
        }

        public List<EffectParameter> GetDebossingColor(ProductDetails productDetails)
        {
            try
            {
                var parameters = this.productColorsMapping.FirstOrDefault(
                    map => map.Key.ProductCode == productDetails.ProductCode
                           && map.Key.ProductColor == productDetails.ProductColor).Value;

                if (parameters == null)
                {
                    parameters = this.productColorsMapping.FirstOrDefault(
                        map => map.Key.ProductCode == productDetails.ProductCode
                               && map.Key.ProductColor == "*").Value;


                    if (parameters == null)
                    {
                        return this.productColorsMapping.FirstOrDefault(
                                map => map.Key.ProductCode == "*" && map.Key.ProductColor == "*")
                            .Value;
                    }
                }
                else
                {
                    return parameters;
                }

                return parameters;
            }
            catch
            {
                return null;
            }
        }

        public List<EffectParameter> GetTexture(ProductDetails productDetails)
        {

            productDetails.ProductCode = string.IsNullOrEmpty(productDetails.ProductCode)
                                             ? string.Empty
                                             : productDetails.ProductCode;
            productDetails.ProductColor = string.IsNullOrEmpty(productDetails.ProductColor)
                                             ? string.Empty
                                             : productDetails.ProductColor;
            productDetails.ProductMaterial = string.IsNullOrEmpty(productDetails.ProductMaterial)
                                             ? string.Empty
                                             : productDetails.ProductMaterial;

            try
            {
                // Check texture for material, color and product
                var textureForMaterialColorProduct = this.productTextureMapping.FirstOrDefault(p => p.Key.ProductMaterial.ToLower() == productDetails.ProductMaterial.ToLower() && p.Key.ProductCode.ToLower() == productDetails.ProductCode.ToLower() && p.Key.ProductColor.ToLower() == productDetails.ProductColor.ToLower());
                if (textureForMaterialColorProduct.Key != null)
                {
                    return textureForMaterialColorProduct.Value;
                }

                // Check texture for material and color
                var textureForMaterialColor = this.productTextureMapping.FirstOrDefault(p => p.Key.ProductMaterial.ToLower() == productDetails.ProductMaterial.ToLower() && p.Key.ProductCode == "*" && p.Key.ProductColor.ToLower() == productDetails.ProductColor.ToLower());
                if (textureForMaterialColor.Key != null)
                {
                    return textureForMaterialColor.Value;
                }

                // Check texture for material
                var textureForMaterial = this.productTextureMapping.FirstOrDefault(p => p.Key.ProductMaterial.ToLower() == productDetails.ProductMaterial.ToLower() && p.Key.ProductCode == "*" && p.Key.ProductColor == "*");
                if (textureForMaterial.Key != null)
                {
                    return textureForMaterial.Value;
                }

                // Default
                return this.productTextureMapping.FirstOrDefault(p => p.Key.ProductMaterial.ToLower() == "*" && p.Key.ProductCode == "*" && p.Key.ProductColor == "*").Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
