namespace lizari.Models.PetModels
{
    public class Product
    {
        public int Id { get; set; }

        // ID товара в XML поставщика
        public int SupplierProductId { get; set; }


        // =========================
        // ОСНОВНАЯ ИНФОРМАЦИЯ
        // заполняем вручную
        // =========================

        public string Title { get; set; } = "";

        public string ShortDescription { get; set; } = "";

        public string Description { get; set; } = "";


        // =========================
        // ЦЕНЫ
        // заполняем вручную
        // =========================

        // Розничная цена
        public decimal Price { get; set; }

        // Оптовая цена
        public decimal WholesalePrice { get; set; }

        // Процент скидки
        public decimal? DiscountPercent { get; set; }

        // Цена со скидкой
        public decimal? DiscountPrice { get; set; }


        // =========================
        // ПРЕИМУЩЕСТВА
        // заполняем вручную
        // =========================

        public string Feature1Title { get; set; } = "";
        public string Feature1Description { get; set; } = "";

        public string Feature2Title { get; set; } = "";
        public string Feature2Description { get; set; } = "";

        public string Feature3Title { get; set; } = "";
        public string Feature3Description { get; set; } = "";

        public string Feature4Title { get; set; } = "";
        public string Feature4Description { get; set; } = "";


        // =========================
        // ХАРАКТЕРИСТИКИ
        // заполняем вручную
        // =========================

        public string Type { get; set; } = "";

        public string Size { get; set; } = "";

        public string Color { get; set; } = "";


        // =========================
        // БЛОК ПОДБОРА
        // заполняем вручную
        // =========================

        public string SelectionTitle { get; set; } = "";

        public string SelectionDescription { get; set; } = "";


        // =========================
        // НАСТРОЙКИ САЙТА
        // =========================

        public bool IsPublished { get; set; }


        // =========================
        // ДАННЫЕ ПОСТАВЩИКА
        // обновляются из XML
        // =========================

        public decimal SupplierPrice { get; set; }

        public bool Available { get; set; }

        public int QuantityInStock { get; set; }

        // ID категории из XML
        public int CategoryId { get; set; }

        public string Picture { get; set; } = "";

        public string VendorCode { get; set; } = "";

        public string Vendor { get; set; } = "";

        public DateTime? SupplierUpdatedAt { get; set; }
    }
}