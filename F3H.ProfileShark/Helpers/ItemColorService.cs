using System.Windows.Media;
using JoeScan.Pinchot;

namespace F3H.ProfileShark.Helpers;

public class ItemColorService 
{
    private readonly Dictionary<HeadCamPair, Color> colors = new()
    {
        { new HeadCamPair(0, Camera.CameraA), Colors.SaddleBrown },
        { new HeadCamPair(0, Camera.CameraB), Colors.Orange },
        { new HeadCamPair(1, Camera.CameraA), Colors.SaddleBrown },
        { new HeadCamPair(1, Camera.CameraB), Colors.Orange }
    };

    public Color GetItemColor(HeadCamPair pair)
    {
        return colors.ContainsKey(pair) ? colors[pair] : Colors.White;
    }

    public Color[] LogColorValues => logColorValues;

    private static readonly Color[] logColorValues =
        Enumerable.Range(0, 256).Select(s => HSL2RGB(0.1, 0.9, s / 255.0)).ToArray();
    
    private static Color HSL2RGB(double h, double sl, double l)
    {
        double v;
        double r, g, b;

        r = l;   // default to gray
        g = l;
        b = l;
        v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
        if (v > 0)
        {
            double m;
            double sv;
            int sextant;
            double fract, vsf, mid1, mid2;

            m = l + l - v;
            sv = (v - m) / v;
            h *= 6.0;
            sextant = (int)h;
            fract = h - sextant;
            vsf = v * sv * fract;
            mid1 = m + vsf;
            mid2 = v - vsf;
            switch (sextant)
            {
                case 0:
                    r = v;
                    g = mid1;
                    b = m;
                    break;
                case 1:
                    r = mid2;
                    g = v;
                    b = m;
                    break;
                case 2:
                    r = m;
                    g = v;
                    b = mid1;
                    break;
                case 3:
                    r = m;
                    g = mid2;
                    b = v;
                    break;
                case 4:
                    r = mid1;
                    g = m;
                    b = v;
                    break;
                case 5:
                    r = v;
                    g = m;
                    b = mid2;
                    break;
            }
        }
        return Color.FromArgb(255, Convert.ToByte(r * 255.0f), Convert.ToByte(g * 255.0f), Convert.ToByte(b * 255.0f));
    }

    public Color[] DistancePalette => plasmaColors;
    private static readonly Color[] plasmaColors = new Color[256]
    {
        Color.FromRgb((byte)13, (byte)8, (byte)135),
        Color.FromRgb((byte)16, (byte)7, (byte)136),
        Color.FromRgb((byte)19, (byte)7, (byte)137),
        Color.FromRgb((byte)22, (byte)7, (byte)138),
        Color.FromRgb((byte)25, (byte)6, (byte)140),
        Color.FromRgb((byte)27, (byte)6, (byte)141),
        Color.FromRgb((byte)29, (byte)6, (byte)142),
        Color.FromRgb((byte)32, (byte)6, (byte)143),
        Color.FromRgb((byte)34, (byte)6, (byte)144),
        Color.FromRgb((byte)36, (byte)6, (byte)145),
        Color.FromRgb((byte)38, (byte)5, (byte)145),
        Color.FromRgb((byte)40, (byte)5, (byte)146),
        Color.FromRgb((byte)42, (byte)5, (byte)147),
        Color.FromRgb((byte)44, (byte)5, (byte)148),
        Color.FromRgb((byte)46, (byte)5, (byte)149),
        Color.FromRgb((byte)47, (byte)5, (byte)150),
        Color.FromRgb((byte)49, (byte)5, (byte)151),
        Color.FromRgb((byte)51, (byte)5, (byte)151),
        Color.FromRgb((byte)53, (byte)4, (byte)152),
        Color.FromRgb((byte)55, (byte)4, (byte)153),
        Color.FromRgb((byte)56, (byte)4, (byte)154),
        Color.FromRgb((byte)58, (byte)4, (byte)154),
        Color.FromRgb((byte)60, (byte)4, (byte)155),
        Color.FromRgb((byte)62, (byte)4, (byte)156),
        Color.FromRgb((byte)63, (byte)4, (byte)156),
        Color.FromRgb((byte)65, (byte)4, (byte)157),
        Color.FromRgb((byte)67, (byte)3, (byte)158),
        Color.FromRgb((byte)68, (byte)3, (byte)158),
        Color.FromRgb((byte)70, (byte)3, (byte)159),
        Color.FromRgb((byte)72, (byte)3, (byte)159),
        Color.FromRgb((byte)73, (byte)3, (byte)160),
        Color.FromRgb((byte)75, (byte)3, (byte)161),
        Color.FromRgb((byte)76, (byte)2, (byte)161),
        Color.FromRgb((byte)78, (byte)2, (byte)162),
        Color.FromRgb((byte)80, (byte)2, (byte)162),
        Color.FromRgb((byte)81, (byte)2, (byte)163),
        Color.FromRgb((byte)83, (byte)2, (byte)163),
        Color.FromRgb((byte)85, (byte)2, (byte)164),
        Color.FromRgb((byte)86, (byte)1, (byte)164),
        Color.FromRgb((byte)88, (byte)1, (byte)164),
        Color.FromRgb((byte)89, (byte)1, (byte)165),
        Color.FromRgb((byte)91, (byte)1, (byte)165),
        Color.FromRgb((byte)92, (byte)1, (byte)166),
        Color.FromRgb((byte)94, (byte)1, (byte)166),
        Color.FromRgb((byte)96, (byte)1, (byte)166),
        Color.FromRgb((byte)97, (byte)0, (byte)167),
        Color.FromRgb((byte)99, (byte)0, (byte)167),
        Color.FromRgb((byte)100, (byte)0, (byte)167),
        Color.FromRgb((byte)102, (byte)0, (byte)167),
        Color.FromRgb((byte)103, (byte)0, (byte)168),
        Color.FromRgb((byte)105, (byte)0, (byte)168),
        Color.FromRgb((byte)106, (byte)0, (byte)168),
        Color.FromRgb((byte)108, (byte)0, (byte)168),
        Color.FromRgb((byte)110, (byte)0, (byte)168),
        Color.FromRgb((byte)111, (byte)0, (byte)168),
        Color.FromRgb((byte)113, (byte)0, (byte)168),
        Color.FromRgb((byte)114, (byte)1, (byte)168),
        Color.FromRgb((byte)116, (byte)1, (byte)168),
        Color.FromRgb((byte)117, (byte)1, (byte)168),
        Color.FromRgb((byte)119, (byte)1, (byte)168),
        Color.FromRgb((byte)120, (byte)1, (byte)168),
        Color.FromRgb((byte)122, (byte)2, (byte)168),
        Color.FromRgb((byte)123, (byte)2, (byte)168),
        Color.FromRgb((byte)125, (byte)3, (byte)168),
        Color.FromRgb((byte)126, (byte)3, (byte)168),
        Color.FromRgb((byte)128, (byte)4, (byte)168),
        Color.FromRgb((byte)129, (byte)4, (byte)167),
        Color.FromRgb((byte)131, (byte)5, (byte)167),
        Color.FromRgb((byte)132, (byte)5, (byte)167),
        Color.FromRgb((byte)134, (byte)6, (byte)166),
        Color.FromRgb((byte)135, (byte)7, (byte)166),
        Color.FromRgb((byte)136, (byte)8, (byte)166),
        Color.FromRgb((byte)138, (byte)9, (byte)165),
        Color.FromRgb((byte)139, (byte)10, (byte)165),
        Color.FromRgb((byte)141, (byte)11, (byte)165),
        Color.FromRgb((byte)142, (byte)12, (byte)164),
        Color.FromRgb((byte)143, (byte)13, (byte)164),
        Color.FromRgb((byte)145, (byte)14, (byte)163),
        Color.FromRgb((byte)146, (byte)15, (byte)163),
        Color.FromRgb((byte)148, (byte)16, (byte)162),
        Color.FromRgb((byte)149, (byte)17, (byte)161),
        Color.FromRgb((byte)150, (byte)19, (byte)161),
        Color.FromRgb((byte)152, (byte)20, (byte)160),
        Color.FromRgb((byte)153, (byte)21, (byte)159),
        Color.FromRgb((byte)154, (byte)22, (byte)159),
        Color.FromRgb((byte)156, (byte)23, (byte)158),
        Color.FromRgb((byte)157, (byte)24, (byte)157),
        Color.FromRgb((byte)158, (byte)25, (byte)157),
        Color.FromRgb((byte)160, (byte)26, (byte)156),
        Color.FromRgb((byte)161, (byte)27, (byte)155),
        Color.FromRgb((byte)162, (byte)29, (byte)154),
        Color.FromRgb((byte)163, (byte)30, (byte)154),
        Color.FromRgb((byte)165, (byte)31, (byte)153),
        Color.FromRgb((byte)166, (byte)32, (byte)152),
        Color.FromRgb((byte)167, (byte)33, (byte)151),
        Color.FromRgb((byte)168, (byte)34, (byte)150),
        Color.FromRgb((byte)170, (byte)35, (byte)149),
        Color.FromRgb((byte)171, (byte)36, (byte)148),
        Color.FromRgb((byte)172, (byte)38, (byte)148),
        Color.FromRgb((byte)173, (byte)39, (byte)147),
        Color.FromRgb((byte)174, (byte)40, (byte)146),
        Color.FromRgb((byte)176, (byte)41, (byte)145),
        Color.FromRgb((byte)177, (byte)42, (byte)144),
        Color.FromRgb((byte)178, (byte)43, (byte)143),
        Color.FromRgb((byte)179, (byte)44, (byte)142),
        Color.FromRgb((byte)180, (byte)46, (byte)141),
        Color.FromRgb((byte)181, (byte)47, (byte)140),
        Color.FromRgb((byte)182, (byte)48, (byte)139),
        Color.FromRgb((byte)183, (byte)49, (byte)138),
        Color.FromRgb((byte)184, (byte)50, (byte)137),
        Color.FromRgb((byte)186, (byte)51, (byte)136),
        Color.FromRgb((byte)187, (byte)52, (byte)136),
        Color.FromRgb((byte)188, (byte)53, (byte)135),
        Color.FromRgb((byte)189, (byte)55, (byte)134),
        Color.FromRgb((byte)190, (byte)56, (byte)133),
        Color.FromRgb((byte)191, (byte)57, (byte)132),
        Color.FromRgb((byte)192, (byte)58, (byte)131),
        Color.FromRgb((byte)193, (byte)59, (byte)130),
        Color.FromRgb((byte)194, (byte)60, (byte)129),
        Color.FromRgb((byte)195, (byte)61, (byte)128),
        Color.FromRgb((byte)196, (byte)62, (byte)127),
        Color.FromRgb((byte)197, (byte)64, (byte)126),
        Color.FromRgb((byte)198, (byte)65, (byte)125),
        Color.FromRgb((byte)199, (byte)66, (byte)124),
        Color.FromRgb((byte)200, (byte)67, (byte)123),
        Color.FromRgb((byte)201, (byte)68, (byte)122),
        Color.FromRgb((byte)202, (byte)69, (byte)122),
        Color.FromRgb((byte)203, (byte)70, (byte)121),
        Color.FromRgb((byte)204, (byte)71, (byte)120),
        Color.FromRgb((byte)204, (byte)73, (byte)119),
        Color.FromRgb((byte)205, (byte)74, (byte)118),
        Color.FromRgb((byte)206, (byte)75, (byte)117),
        Color.FromRgb((byte)207, (byte)76, (byte)116),
        Color.FromRgb((byte)208, (byte)77, (byte)115),
        Color.FromRgb((byte)209, (byte)78, (byte)114),
        Color.FromRgb((byte)210, (byte)79, (byte)113),
        Color.FromRgb((byte)211, (byte)81, (byte)113),
        Color.FromRgb((byte)212, (byte)82, (byte)112),
        Color.FromRgb((byte)213, (byte)83, (byte)111),
        Color.FromRgb((byte)213, (byte)84, (byte)110),
        Color.FromRgb((byte)214, (byte)85, (byte)109),
        Color.FromRgb((byte)215, (byte)86, (byte)108),
        Color.FromRgb((byte)216, (byte)87, (byte)107),
        Color.FromRgb((byte)217, (byte)88, (byte)106),
        Color.FromRgb((byte)218, (byte)90, (byte)106),
        Color.FromRgb((byte)218, (byte)91, (byte)105),
        Color.FromRgb((byte)219, (byte)92, (byte)104),
        Color.FromRgb((byte)220, (byte)93, (byte)103),
        Color.FromRgb((byte)221, (byte)94, (byte)102),
        Color.FromRgb((byte)222, (byte)95, (byte)101),
        Color.FromRgb((byte)222, (byte)97, (byte)100),
        Color.FromRgb((byte)223, (byte)98, (byte)99),
        Color.FromRgb((byte)224, (byte)99, (byte)99),
        Color.FromRgb((byte)225, (byte)100, (byte)98),
        Color.FromRgb((byte)226, (byte)101, (byte)97),
        Color.FromRgb((byte)226, (byte)102, (byte)96),
        Color.FromRgb((byte)227, (byte)104, (byte)95),
        Color.FromRgb((byte)228, (byte)105, (byte)94),
        Color.FromRgb((byte)229, (byte)106, (byte)93),
        Color.FromRgb((byte)229, (byte)107, (byte)93),
        Color.FromRgb((byte)230, (byte)108, (byte)92),
        Color.FromRgb((byte)231, (byte)110, (byte)91),
        Color.FromRgb((byte)231, (byte)111, (byte)90),
        Color.FromRgb((byte)232, (byte)112, (byte)89),
        Color.FromRgb((byte)233, (byte)113, (byte)88),
        Color.FromRgb((byte)233, (byte)114, (byte)87),
        Color.FromRgb((byte)234, (byte)116, (byte)87),
        Color.FromRgb((byte)235, (byte)117, (byte)86),
        Color.FromRgb((byte)235, (byte)118, (byte)85),
        Color.FromRgb((byte)236, (byte)119, (byte)84),
        Color.FromRgb((byte)237, (byte)121, (byte)83),
        Color.FromRgb((byte)237, (byte)122, (byte)82),
        Color.FromRgb((byte)238, (byte)123, (byte)81),
        Color.FromRgb((byte)239, (byte)124, (byte)81),
        Color.FromRgb((byte)239, (byte)126, (byte)80),
        Color.FromRgb((byte)240, (byte)127, (byte)79),
        Color.FromRgb((byte)240, (byte)128, (byte)78),
        Color.FromRgb((byte)241, (byte)129, (byte)77),
        Color.FromRgb((byte)241, (byte)131, (byte)76),
        Color.FromRgb((byte)242, (byte)132, (byte)75),
        Color.FromRgb((byte)243, (byte)133, (byte)75),
        Color.FromRgb((byte)243, (byte)135, (byte)74),
        Color.FromRgb((byte)244, (byte)136, (byte)73),
        Color.FromRgb((byte)244, (byte)137, (byte)72),
        Color.FromRgb((byte)245, (byte)139, (byte)71),
        Color.FromRgb((byte)245, (byte)140, (byte)70),
        Color.FromRgb((byte)246, (byte)141, (byte)69),
        Color.FromRgb((byte)246, (byte)143, (byte)68),
        Color.FromRgb((byte)247, (byte)144, (byte)68),
        Color.FromRgb((byte)247, (byte)145, (byte)67),
        Color.FromRgb((byte)247, (byte)147, (byte)66),
        Color.FromRgb((byte)248, (byte)148, (byte)65),
        Color.FromRgb((byte)248, (byte)149, (byte)64),
        Color.FromRgb((byte)249, (byte)151, (byte)63),
        Color.FromRgb((byte)249, (byte)152, (byte)62),
        Color.FromRgb((byte)249, (byte)154, (byte)62),
        Color.FromRgb((byte)250, (byte)155, (byte)61),
        Color.FromRgb((byte)250, (byte)156, (byte)60),
        Color.FromRgb((byte)250, (byte)158, (byte)59),
        Color.FromRgb((byte)251, (byte)159, (byte)58),
        Color.FromRgb((byte)251, (byte)161, (byte)57),
        Color.FromRgb((byte)251, (byte)162, (byte)56),
        Color.FromRgb((byte)252, (byte)163, (byte)56),
        Color.FromRgb((byte)252, (byte)165, (byte)55),
        Color.FromRgb((byte)252, (byte)166, (byte)54),
        Color.FromRgb((byte)252, (byte)168, (byte)53),
        Color.FromRgb((byte)252, (byte)169, (byte)52),
        Color.FromRgb((byte)253, (byte)171, (byte)51),
        Color.FromRgb((byte)253, (byte)172, (byte)51),
        Color.FromRgb((byte)253, (byte)174, (byte)50),
        Color.FromRgb((byte)253, (byte)175, (byte)49),
        Color.FromRgb((byte)253, (byte)177, (byte)48),
        Color.FromRgb((byte)253, (byte)178, (byte)47),
        Color.FromRgb((byte)253, (byte)180, (byte)47),
        Color.FromRgb((byte)253, (byte)181, (byte)46),
        Color.FromRgb((byte)254, (byte)183, (byte)45),
        Color.FromRgb((byte)254, (byte)184, (byte)44),
        Color.FromRgb((byte)254, (byte)186, (byte)44),
        Color.FromRgb((byte)254, (byte)187, (byte)43),
        Color.FromRgb((byte)254, (byte)189, (byte)42),
        Color.FromRgb((byte)254, (byte)190, (byte)42),
        Color.FromRgb((byte)254, (byte)192, (byte)41),
        Color.FromRgb((byte)253, (byte)194, (byte)41),
        Color.FromRgb((byte)253, (byte)195, (byte)40),
        Color.FromRgb((byte)253, (byte)197, (byte)39),
        Color.FromRgb((byte)253, (byte)198, (byte)39),
        Color.FromRgb((byte)253, (byte)200, (byte)39),
        Color.FromRgb((byte)253, (byte)202, (byte)38),
        Color.FromRgb((byte)253, (byte)203, (byte)38),
        Color.FromRgb((byte)252, (byte)205, (byte)37),
        Color.FromRgb((byte)252, (byte)206, (byte)37),
        Color.FromRgb((byte)252, (byte)208, (byte)37),
        Color.FromRgb((byte)252, (byte)210, (byte)37),
        Color.FromRgb((byte)251, (byte)211, (byte)36),
        Color.FromRgb((byte)251, (byte)213, (byte)36),
        Color.FromRgb((byte)251, (byte)215, (byte)36),
        Color.FromRgb((byte)250, (byte)216, (byte)36),
        Color.FromRgb((byte)250, (byte)218, (byte)36),
        Color.FromRgb((byte)249, (byte)220, (byte)36),
        Color.FromRgb((byte)249, (byte)221, (byte)37),
        Color.FromRgb((byte)248, (byte)223, (byte)37),
        Color.FromRgb((byte)248, (byte)225, (byte)37),
        Color.FromRgb((byte)247, (byte)226, (byte)37),
        Color.FromRgb((byte)247, (byte)228, (byte)37),
        Color.FromRgb((byte)246, (byte)230, (byte)38),
        Color.FromRgb((byte)246, (byte)232, (byte)38),
        Color.FromRgb((byte)245, (byte)233, (byte)38),
        Color.FromRgb((byte)245, (byte)235, (byte)39),
        Color.FromRgb((byte)244, (byte)237, (byte)39),
        Color.FromRgb((byte)243, (byte)238, (byte)39),
        Color.FromRgb((byte)243, (byte)240, (byte)39),
        Color.FromRgb((byte)242, (byte)242, (byte)39),
        Color.FromRgb((byte)241, (byte)244, (byte)38),
        Color.FromRgb((byte)241, (byte)245, (byte)37),
        Color.FromRgb((byte)240, (byte)247, (byte)36),
        Color.FromRgb((byte)240, (byte)249, (byte)33)
    };
}