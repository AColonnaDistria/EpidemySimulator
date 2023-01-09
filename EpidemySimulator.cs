using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace VirusSimulator
{
    class EpidemySimulator
    {
        List<Agent> agents;
        Random rnd;

        Virus virus;

        double bmpWidth;
        double bmpHeight;

        double spWidth;
        double spHeight;

        Stats stats;
        Grid grid;

        public List<Agent> AgentsList
        {
            get
            {
                return agents;
            }
        }
        public double SpaceWidth
        {
            get
            {
                return spWidth;
            }
        }
        public double SpaceHeight
        {
            get
            {
                return spHeight;
            }
        }
        public Virus VirusProp
        {
            get
            {
                return virus;
            }
        }

        public EpidemySimulator(int w, int h, double sw, double sh, int numberOfAgents, int numberOfInfected, Virus vir)
        {
            rnd = new Random();

            agents = new List<Agent>();

            bmpWidth = (double)w;
            bmpHeight = (double)h;

            spWidth = sw;
            spHeight = sh;

            setVirus(vir);
            for (int id = 0; id < numberOfAgents; ++id)
            {
                agents.Add(new Agent());
                agents[id].id = id;
                agents[id].x = rnd.NextDouble() * spWidth;
                agents[id].y = rnd.NextDouble() * spHeight;

                double speed = 4.0;

                double dx = rnd.NextDouble() * speed * 2.0 - speed; // from -2.0 to 2.0
                double dy = Math.Sqrt(speed * speed - dx * dx);
                if (rnd.Next(2) == 0)
                {
                    dy *= -1.0;
                }

                agents[id].dx = dx;
                agents[id].dy = dy;

                agents[id].state = STATE.healthy;

                int caseX = (int)(agents[id].x / grid.Length);
                int caseY = (int)(agents[id].y / grid.Length);
                grid.ListFromXY(caseX, caseY).Add(id);
            }

            for (int id = 0; id < numberOfInfected; ++id)
            {
                agents[id].state = STATE.infected;
            }

            stats.day = 0;

            stats.healthyCount = numberOfAgents - numberOfInfected;
            stats.infectedCount = numberOfInfected;
            stats.immuneCount = 0;
            stats.deadCount = 0;

            stats.populationSize = numberOfAgents;
            stats.maxInfected = 0;
            stats.maxInfectedDay = 0;
        }

        public void setVirus(Virus vir)
        {
            double length = vir.contaminationRayon / Math.Sqrt(2.0);

            if (length > 0.0)
            {
                virus.CloneFrom(vir);

                int width = (int)Math.Ceiling(spWidth / length);
                int height = (int)Math.Ceiling(spHeight / length);
                grid = new Grid(width, height, length);
            }
        }

        public void CalculateContacts()
        {
            stats.totalContactsInInfectedExcludeAlreadyInfected = 0;

            double r = virus.contaminationRayon;

            for (int x = 0; x < grid.Width; ++x)
            {
                for (int y = 0; y < grid.Height; ++y)
                {
                    List<int> agentsInCase = grid.ListFromXY(x, y);
                    for (int i = 0; i < agentsInCase.Count; ++i)
                    {
                        int id = agentsInCase[i];

                        if (agents[id].tempState == STATE.infected)
                        {
                            for (int i2 = 0; i2 < agentsInCase.Count; ++i2)
                            {
                                int id2 = agentsInCase[i2];

                                if (agents[id2].state == STATE.healthy)
                                {
                                    double p = rnd.NextDouble();

                                    if (p < virus.contaminationProbability)
                                    {
                                        agents[id2].state = STATE.infected;

                                        stats.infectedCount += 1;
                                        stats.healthyCount -= 1;
                                    }
                                    else
                                    {
                                        stats.totalContactsInInfectedExcludeAlreadyInfected += 1;
                                    }
                                }
                            }

                            for (int xDelta = -2; xDelta <= 2; ++xDelta)
                            {
                                for (int yDelta = -2; yDelta <= 2; ++yDelta)
                                {
                                    if ((xDelta != 0 || yDelta != 0) && (Math.Abs(xDelta) + Math.Abs(yDelta) < 4) && (xDelta + x >= 0 && yDelta + y >= 0 && xDelta + x < grid.Width && yDelta + y < grid.Height))
                                    {
                                        agentsInCase = grid.ListFromXY(x + xDelta, y + yDelta);
                                        
                                        for (int i2 = 0; i2 < agentsInCase.Count; ++i2)
                                        {
                                            int id2 = agentsInCase[i2];
                                            double distanceSquared = (agents[id].x - agents[id2].x) * (agents[id].x - agents[id2].x) + (agents[id].y - agents[id2].y) * (agents[id].y - agents[id2].y);

                                            if (distanceSquared <= r * r && id != id2 && agents[id2].state == STATE.healthy)
                                            {
                                                double p = rnd.NextDouble();

                                                if (p < virus.contaminationProbability)
                                                {
                                                    agents[id2].state = STATE.infected;

                                                    stats.infectedCount += 1;
                                                    stats.healthyCount -= 1;
                                                }
                                                else
                                                {
                                                    stats.totalContactsInInfectedExcludeAlreadyInfected += 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void Next()
        {
            grid.Clear();

            int count = agents.Count;
            int id = 0;
            while (id < count)
            {
                agents[id].tempState = agents[id].state;

                bool isRemoved = false;
                if (agents[id].state == STATE.immune)
                {
                    double p = rnd.NextDouble();

                    if (p < virus.immunityLostRate)
                    {
                        agents[id].state = STATE.healthy;
                        agents[id].tempState = agents[id].state;

                        stats.immuneCount -= 1;
                        stats.healthyCount += 1;
                    }
                }
                else if (agents[id].state == STATE.infected)
                {
                    double p = rnd.NextDouble();

                    if (p < virus.cureProbability)
                    {
                        double pImmune = rnd.NextDouble();

                        if (pImmune < virus.immuneProbability)
                        {
                            agents[id].state = STATE.immune;
                            agents[id].tempState = agents[id].state;

                            stats.immuneCount += 1;
                            stats.infectedCount -= 1;
                        }
                        else
                        {
                            agents[id].state = STATE.healthy;
                            agents[id].tempState = agents[id].state;

                            stats.healthyCount += 1;
                            stats.infectedCount -= 1;
                        }
                    }
                    else if (p < virus.cureProbability + virus.deathProbability)
                    {
                        agents[id].state = STATE.dead;
                        agents[id].tempState = agents[id].state;

                        isRemoved = true;
                        agents.RemoveAt(id);
                        --count;
                        --id;

                        stats.deadCount += 1;
                        stats.infectedCount -= 1;
                    }
                }

                if (!isRemoved)
                {
                    double lg = Math.Sqrt((agents[id].dx) * (agents[id].dx) + (agents[id].dy) * (agents[id].dy));

                    double alpha = Math.Acos(agents[id].dx / lg);

                    alpha += 2 * Math.PI * 0.05 * (rnd.NextDouble() - 0.5);

                    double dx2 = Math.Cos(alpha) * (lg);
                    double dy2 = Math.Sin(alpha) * (lg);

                    if (agents[id].dy < 0)
                    {
                        dy2 *= -1.0;
                    }

                    agents[id].dx = dx2;
                    agents[id].dy = dy2;

                    double x2 = agents[id].x + agents[id].dx;
                    double y2 = agents[id].y + agents[id].dy;

                    if (x2 < 0)
                    {
                        agents[id].dx *= -1;
                        agents[id].x = 1.0;
                    }
                    if (y2 < 0)
                    {
                        agents[id].dy *= -1;
                        agents[id].y = 1.0;
                    }
                    if (x2 >= spWidth)
                    {
                        agents[id].dx *= -1;
                        agents[id].x = spWidth - 1.0;
                    }
                    if (y2 >= spHeight)
                    {
                        agents[id].dy *= -1;
                        agents[id].y = spHeight - 1.0;
                    }

                    x2 = agents[id].x + agents[id].dx;
                    y2 = agents[id].y + agents[id].dy;

                    agents[id].x = x2;
                    agents[id].y = y2;

                    if ((agents[id].state == STATE.infected || agents[id].state == STATE.healthy))
                    {
                        int caseX = (int)(agents[id].x / grid.Length);
                        int caseY = (int)(agents[id].y / grid.Length);
                        grid.ListFromXY(caseX, caseY).Add(id);
                    }
                }

                ++id;
            }

            CalculateContacts();

            if (stats.infectedCount >= stats.maxInfected)
            {
                stats.maxInfected = stats.infectedCount;
                stats.maxInfectedDay = stats.day;
            }
            stats.day += 1;
        }

        public Stats getStats()
        {
            return stats;
        }

        public enum STATE
        {
            healthy = 0,
            infected = 1,
            immune = 2,
            dead = 3,
        }

        public struct Stats
        {
            public int day;

            public int populationSize;

            public int healthyCount;
            public int infectedCount;
            public int immuneCount;
            public int deadCount;

            public int totalContactsInInfectedExcludeAlreadyInfected;

            public int maxInfected;
            public int maxInfectedDay;

            public string getString()
            {
                return "Day: " + (this.day.ToString()) + ", Healthy: " + (this.healthyCount + this.immuneCount).ToString() + ", Infected: " + this.infectedCount.ToString() + ", Immune: " + this.immuneCount.ToString() + ", Dead: " + this.deadCount.ToString() + "\nTotal contacts in infected susceptible to be infected: " + this.totalContactsInInfectedExcludeAlreadyInfected.ToString() + "\nInfection peak: " + this.maxInfected.ToString() + ", Infection peak day: " + this.maxInfectedDay.ToString();
            }
        }
        public class Agent
        {
            public double x;
            public double y;

            public double dx;
            public double dy;

            public STATE state;
            public STATE tempState;

            public int id;
        }
        public struct Virus
        {
            public double contaminationProbability;
            public double cureProbability;
            public double deathProbability;
            public double immuneProbability;
            public double contaminationRayon;
            public double immunityLostRate;

            public void CloneFrom(Virus virus2)
            {
                contaminationProbability = virus2.contaminationProbability;
                cureProbability = virus2.cureProbability;
                deathProbability = virus2.deathProbability;
                immuneProbability = virus2.immuneProbability;
                contaminationRayon = virus2.contaminationRayon;
                immunityLostRate = virus2.immunityLostRate;
            }
            private string str(double d)
            {
                string s = d.ToString().Replace(',', '.');

                if (!s.Contains('.'))
                {
                    s += ".0";
                }
                return s;
            }
            private bool isAcceptableDouble(string s)
            {
                for (int i = 0; i < s.Length; ++i)
                {
                    if (!((s[i] <= '9' && s[i] >= '0') || (s[i] == '.') || (s[i] == '-' && i == 0)))
                    {
                        return false;
                    }
                }
                return true;
            }
            private double convertToDouble(string s)
            {
                double dbsup = 0.0;
                double sgn = 1.0;

                int j = -1;
                for (int i = 0; i < s.Length; ++i)
                {
                    if (i == 0 && s[i] == '-')
                    {
                        sgn = -1.0;
                    }
                    else
                    {
                        if (s[i] >= '0' && s[i] <= '9')
                        {
                            dbsup *= 10.0;
                            dbsup += (double)((int)(s[i] - '0'));
                        }
                        else if (s[i] == '.')
                        {
                            j = i;
                            break;
                        }
                    }
                }

                double dbinf = 0.0;
                if (j != -1)
                {
                    for (int i = s.Length - 1; i >= j + 1; --i)
                    {
                        dbinf += (double)((int)(s[i] - '0'));
                        dbinf /= 10.0;
                    }
                }

                double db = dbinf + dbsup;
                db *= sgn;

                return db;
            }
            public string getString()
            {
                return "{contaminationProbability: " + str(this.contaminationProbability) + "\ncureProbability: " + str(this.cureProbability) + "\ndeathProbability: " + str(this.deathProbability) + "\nimmuneProbability: " + str(this.immuneProbability) + "\ncontaminationRayon: " + str(this.contaminationRayon) + "\nimmunityLostRate: " + str(this.immunityLostRate) + "}";
            }
        }

        public class Grid
        {
            double length;
            int width;
            int height;
            List<int>[,] grid;

            public Grid(int width, int height, double length)
            {
                grid = new List<int>[width, height];

                int c = 0;
                for (int i = 0; i < width; ++i)
                {
                    for (int j = 0; j < height; ++j)
                    {
                        grid[i, j] = new List<int>();
                    }
                }

                this.width = width;
                this.height = height;
                this.length = length;
            }
            public void Clear()
            {
                for (int i = 0; i < width; ++i)
                {
                    for (int j = 0; j < height; ++j)
                    {
                        grid[i, j].Clear();
                    }
                }
            }
            public List<int> ListFromXY(int x, int y)
            {
                return grid[x, y];
            }
            public int Width
            {
                get
                {
                    return width;
                }
            }
            public int Height
            {
                get
                {
                    return height;
                }
            }
            public double Length
            {
                get
                {
                    return length;
                }
            }
        }
    }
}
