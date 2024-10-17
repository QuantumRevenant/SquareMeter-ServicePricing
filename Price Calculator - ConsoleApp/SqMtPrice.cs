using System.Reflection;
using System.Text.Json;
class SqMtPrice
{
    public static double PricePerSquareMeter { get; set; }
    public static double ValuePerSquareMeterPerPost { get; set; }
    public static double PostsBeforeDiscountReduction { get; set; }
    public static double InitialDiscount { get; set; }
    public static double DiscountReduction { get; set; }
    public static List<SpecialPost> SpecialPosts = new List<SpecialPost>();
    public class SpecialPost
    {
        public SpecialPost(double SquareMeters,double Discount)
        {
            this.SquareMeters=SquareMeters;
            this.Discount=Discount;
        }

        public double SquareMeters { get; set; }
        public double Discount { get; set; }
    }


    public static void LoadValues()
    {
        // Leer el recurso embebido
        string jsonString;
        var assembly = Assembly.GetExecutingAssembly();

        using (Stream stream = assembly.GetManifestResourceStream("Price_Calculator___ConsoleApp.config.json"))
        using (StreamReader reader = new StreamReader(stream))
        {
            jsonString = reader.ReadToEnd();
        }

        // Parsear el JSON
        using (JsonDocument doc = JsonDocument.Parse(jsonString))
        {
            JsonElement root = doc.RootElement;

            // Asignar valores directamente
            PricePerSquareMeter = root.GetProperty("PricePerSquareMeter").GetDouble();
            ValuePerSquareMeterPerPost = root.GetProperty("ValuePerSquareMeterPerPost").GetDouble();
            PostsBeforeDiscountReduction = root.GetProperty("PostsBeforeDiscountReduction").GetDouble();
            InitialDiscount = root.GetProperty("InitialDiscount").GetDouble();
            DiscountReduction = root.GetProperty("DiscountReduction").GetDouble();

            // Limpiar y actualizar la lista de SpecialPosts
            SpecialPosts.Clear();
            foreach (JsonElement post in root.GetProperty("SpecialPosts").EnumerateArray())
            {
                double squareMeters = post.GetProperty("SquareMeters").GetDouble();
                double discount = post.GetProperty("Discount").GetDouble();
                SpecialPosts.Add(new SpecialPost(squareMeters, discount));
            }
        }

        Console.WriteLine("Configuración cargada exitosamente.");
    } 

    #region Regular Posts
    public static int DividingPattern(double squareMeters)
    {
        if (squareMeters <= 0) return -1;

        int value = (int)(squareMeters / ValuePerSquareMeterPerPost);

        return (squareMeters % ValuePerSquareMeterPerPost == 0 && squareMeters != 0) ? value - 1 : value;
    }
    public static double GetPreviousRegularPost(double squareMeters)
    {
        return ValuePerSquareMeterPerPost * DividingPattern(squareMeters);
    }
    public static double GetRegularPost(double squareMeters)
    {
        return ValuePerSquareMeterPerPost * (DividingPattern(squareMeters) + 1);
    }
    public static double GetNextRegularPost(double squareMeters)
    {
        return ValuePerSquareMeterPerPost * (DividingPattern(squareMeters) + 2);
    }
    #endregion

    #region Special Posts
    public static double GetNearestSpecialPostBack(double squareMeters)
    {
        double nearestPost = double.NegativeInfinity;

        foreach (SpecialPost specialPost in SpecialPosts)
            if (specialPost.SquareMeters < squareMeters && specialPost.SquareMeters > nearestPost)
                nearestPost = specialPost.SquareMeters;

        return nearestPost;
    }

    public static double GetNearestSpecialPostFront(double squareMeters)
    {
        double nearestPost = double.PositiveInfinity;

        foreach (SpecialPost specialPost in SpecialPosts)
            if (specialPost.SquareMeters > squareMeters && specialPost.SquareMeters < nearestPost)
                nearestPost = specialPost.SquareMeters;

        return nearestPost;
    }

    #endregion

    #region Final Posts
    public static double GetPreviousPost(double squareMeters)
    {
        double previousRegularPost = GetPreviousRegularPost(squareMeters);
        double nearestSpecialPostBack = GetNearestSpecialPostBack(squareMeters);

        return previousRegularPost > nearestSpecialPostBack ? previousRegularPost : nearestSpecialPostBack;
    }

    public static double GetActualPost(double squareMeters)
    {
        double regularPost = GetRegularPost(squareMeters);
        double nearestSpecialPostFront = GetNearestSpecialPostFront(squareMeters);

        if(SpecialPosts?.FirstOrDefault(post=>post.SquareMeters==squareMeters)!=null) return squareMeters;

        return regularPost < nearestSpecialPostFront ? regularPost : nearestSpecialPostFront;
    }

    public static double GetNextPost(double squareMeters)
    {
        double actualPost = GetActualPost(squareMeters);

        double ap_RegularPost = actualPost % ValuePerSquareMeterPerPost == 0 ? GetNextRegularPost(actualPost) : GetRegularPost(actualPost);
        double ap_NearestSpecialPostFront = GetNearestSpecialPostFront(actualPost);

        return ap_RegularPost < ap_NearestSpecialPostFront ? ap_RegularPost : ap_NearestSpecialPostFront;
    }

    #endregion
    
    #region Discounts
    public static double GetRegularDiscount(double squareMeters)
    {
        double numerator=squareMeters;
        double denominator=PostsBeforeDiscountReduction*ValuePerSquareMeterPerPost;

        int power=(int)(numerator/denominator);

        return InitialDiscount / Math.Pow(DiscountReduction, power);
    }

    public static double? GetSpecialDiscount(double squareMeters)
    {
        if (squareMeters == 0) return 0;
        return SpecialPosts?.FirstOrDefault(post => post.SquareMeters == squareMeters)?.Discount;
    }

    public static double GetDiscount(double squareMeters)
    {
        double actualPost = GetActualPost(squareMeters);
        double? specialDiscount = GetSpecialDiscount(actualPost);

        if (specialDiscount == null)
            return GetRegularDiscount(actualPost);
        else
            return (double)specialDiscount;
    }
    #endregion
    public static double GetPricePerSquareMeter(double squareMeters)
    {
        if (squareMeters <= 0)
            return PricePerSquareMeter;


        return GetPricePerSquareMeter(GetPreviousPost(squareMeters)) * ((100 - GetDiscount(squareMeters)) / 100);
    }

    public static double GetTotalPrice(double squareMeters)
    {
        return GetActualPost(squareMeters) * GetPricePerSquareMeter(squareMeters);
    }
}