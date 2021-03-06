﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MegaDesk_Razor.Models
{
    public enum Material
    {
        [Display(Name = "Laminate")]
        Laminate = 100,
        [Display(Name = "Oak")]
        Oak = 200,
        [Display(Name = "Rosewood")]
        Rosewood = 300,
        [Display(Name = "Veneer")]
        Veneer = 125,
        [Display(Name = "Pine")]
        Pine = 50
    };

    public enum Drawers
    {
        [Display(Name = "1 Drawer")]
        One = 50,
        [Display(Name = "2 Drawers")]
        Two = 100,
        [Display(Name = "3 Drawers")]
        Three = 150,
        [Display(Name = "4 Drawers")]
        Four = 200,
        [Display(Name = "5 Drawers")]
        Five = 250,
        [Display(Name = "6 Drawers")]
        Six = 300,
        [Display(Name = "7 Drawers")]
        Seven = 350
    };

    public enum RushDays
    {
        [Display(Name = "No Rush")]
        None = 0,

        [Display(Name = "3 Days")]
        ThreeDay = 3,
        
        [Display(Name = "5 Days")]
        FiveDay = 5,
        
        [Display(Name = "7 Days")]
        SevenDay = 7
    };

    public class DeskQuote
    {
        // primary key for database
        public int DeskQuoteID { get; set; }

        // deskQuote Variables
        [Display(Name = "Customer Name")]
        [Required]
        public string CustomerName { get; set; }

        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime OrderDate { get; set; }

        // desk variables
        [Display(Name = "Rush Days")]
        [Required]
        [EnumDataType(typeof(RushDays))]
        public RushDays RushDays { get; set; }

        [Display(Name = "Drawers")]
        [Required]
        [EnumDataType(typeof(Drawers))]
        public Drawers Drawers { get; set; }

        [Display(Name = "Material")]
        [Required]
        [EnumDataType(typeof(Material))]
        public Material DeskMaterial { get; set; }

        [Display(Name = "Width")]
        [Range(24,96)]
        [Required]
        public int Width { get; set; }

        [Display(Name = "Length")]
        [Range(12,48)]
        [Required]
        public int Length { get; set; }

        [Display(Name = "Total Cost")]
        [Required]
        public int QuoteTotal { get; set; }

        // private constant variables
        private const int PRICE_BASE = 200;
        private const int SIZE_THRESHOLD = 1000;
        private const int PRICE_SURFACEAREA = 1;
        private const int PRICE_DRAWER = 50;

        // calculated variables
        private int SurfaceArea
        {
            get
            {
                return Width * Length;
            }
        }
        public int TotalCost
        {
            get
            {
                //int quoteTotal = CalculateQuoteTotal(SurfaceArea, (int)Rush, (int)DeskMaterial);
                int quoteTotal = PRICE_BASE + DrawerCost() + SurfaceArea + (int)RushDays+ (int)DeskMaterial;
                return quoteTotal;
            }

        }
        private int MatCost
        {
            get
            {
                return (int)DeskMaterial;
            }
        }

        // FUNCTIONS
       /* private int CalculateQuoteTotal(int surfaceArea, int rushDays, int matCost)
        {
            return PRICE_BASE + DrawerCost()+ matCost
                + RushCost(surfaceArea, rushDays) + SurfaceAreaCost(surfaceArea);
        }
        */
        // surface area cost calculation
        private int SurfaceAreaCost(int size)
        {
            if (size < 1000)
                return 0;
            else if (size > 1000)
                return size - 1000;
            else
                return 1;
        }
        // calculate cost of drawers
        private int DrawerCost()
        {
            return (int)Drawers;
        }
        
        // Calculate Rush Cost
        private int RushCost(int surfaceArea, int days)
        {
            StreamReader reader = new StreamReader(@"rushOrder.txt");

            // James' Attempt
            int[,] priceMap = new int[3, 3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Int32.TryParse(reader.ReadLine(), out priceMap[i, j]);
                }
            }
            reader.Close();

            switch (days)
            {
                case 3:
                    if (surfaceArea < 1000)
                        return priceMap[0, 0];
                    else if (surfaceArea >= 1000 && surfaceArea <= 2000)
                        return priceMap[0, 1];
                    else if (surfaceArea > 2000)
                        return priceMap[0, 2];
                    break;
                case 5:
                    if (surfaceArea < 1000)
                        return priceMap[1, 0];
                    else if (surfaceArea >= 1000 && surfaceArea <= 2000)
                        return priceMap[1, 1];
                    else if (surfaceArea > 2000)
                        return priceMap[1, 2];
                    break;
                case 7:
                    if (surfaceArea < 1000)
                        return priceMap[2, 0];
                    else if (surfaceArea >= 1000 && surfaceArea <= 2000)
                        return priceMap[2, 1];
                    else if (surfaceArea > 2000)
                        return priceMap[2, 2];
                    break;
                case 0:
                    return 0;
                default:
                    break;
            }
            return 0;
        }
    }
}
