using System.Security.Policy;

namespace BaesianNetworks {
    public class Driver {
        
        // Runs tests on the test.bif file for debugging purposes.
        public void DebugRunTests() { 
            // VariableElimination VEsolver = new VEsolver();
            // Gibbs GibsSolver = new GibsSolver();
        
            // Test Network
            BaesNetwork test = new BaesNetwork("test.bif");
            // No Evidence
            // VEsolver.solve(""); 
            // GibsSolver.solve();
            // Little Evidence ---------------------
            // VEsolver.solve(); 
            // GibsSolver.solve();
            // Moderate Evidence ----------------------
            // VEsolver.solve(); 
            // GibsSolver.solve();
        }
        
        
        // Runs all the tests provided by the Project Document.
        // There are three tests for each network generally. Tests
        // including no evidence, little evidence, and moderate evidence.
        // Each network performs these test and the results are compared.
        public void RunTests() {
            // VariableElimination VEsolver = new VEsolver();
            // Gibbs GibsSolver = new GibsSolver();
            
            // Alarm Network [inputs passed]
            // TODO allow query variables to be queried without any evidence
            BaesNetwork alarm = new BaesNetwork("alarm.bif");
            // No Evidence -----------------------
            // VEsolver.solve("HYPOVOLEMIA", alarm); 
            // GibsSolver.solve("HYPOVOLEMIA", alarm);
            // VEsolver.solve("LVFAILURE", alarm); 
            // GibsSolver.solve("LVFAILURE", alarm);
            // VEsolver.solve("ERRLOWOUTPUT", alarm);
            // GibsSolver.solve("ERRLOWOUTPUT", alarm);
            // Little Evidence --------------------
            string LE_alarm = "|HRBP=HIGH,CO=LOW,BP=HIGH";
            // VEsolver.solve("HYPOVOLEMIA" + LE_alarm, alarm); 
            // GibsSolver.solve("HYPOVOLEMIA" + LE_alarm, alarm);
            // VEsolver.solve("LVFAILURE" + LE_alarm, alarm); 
            // GibsSolver.solve("LVFAILURE" + LE_alarm, alarm);
            // VEsolver.solve("ERRLOWOUTPUT" + LE_alarm, alarm);
            // GibsSolver.solve("ERRLOWOUTPUT" + LE_alarm, alarm);
            // Moderate Evidence -------------------
            string MO_alarm = LE_alarm + ",HRSAT=LOW,HREKG=LOW,HISTORY=TRUE";
            // VEsolver.solve("HYPOVOLEMIA" + MO_alarm, alarm); 
            // GibsSolver.solve("HYPOVOLEMIA" + MO_alarm, alarm);
            // VEsolver.solve("LVFAILURE" + MO_alarm, alarm); 
            // GibsSolver.solve("LVFAILURE" + MO_alarm, alarm);
            // VEsolver.solve("ERRLOWOUTPUT" + MO_alarm, alarm);
            // GibsSolver.solve("ERRLOWOUTPUT" + MO_alarm, alarm)
            
            // Child Network [test inputs still]
            // TODO: fix the parser to account for tests in " " 
            BaesNetwork child = new BaesNetwork("child.bif");
            // No Evidence
            // VEsolver.solve("Disease", child); 
            // GibsSolver.solve("Disease", child);
            // Little Evidence ----------------------
            string LE_child = "|LowerBodyO2=“<5”,RUQO2=“>=12”,CO2Report=“>=7.5”,XrayReport=Asy/Patchy";
            // VEsolver.solve("Disease" + LE_child, child);
            // GibsSolver.solve("Disease" + LE_child, child);
            // Moderate Evidence -------------------------
            string MO_child = LE_child + ",GruntingReport=Yes,LVHReport=Yes,Age=“11-30 Days”";
            // VEsolver.solve("Disease" + MO_child, child);
            // GibsSolver.solve("Disease" + MO_child, child);
            
            // Hailfinder Network [inputs passed]
            BaesNetwork hailfinder = new BaesNetwork("hailfinder.bif");
            // No Evidence
            // VEsolver.solve("SatContMoist", hailfinder); 
            // GibsSolver.solve("SatContMoist", hailfinder);
            // VEsolver.solve("LLIW", hailfinder); 
            // GibsSolver.solve("LLIW", hailfinder);
            // Little Evidence -------------------------------
            string LE_hailfinder = "|R5Fcst=XNIL,N34StarFcst=XNIL,MountainFcst=XNIL,AreaMoDryAir=VeryWet";
            // VEsolver.solve("SatContMoist" + LE_hailfinder, hailfinder); 
            // GibsSolver.solve("SatContMoist" + LE_hailfinder, hailfinder);
            // VEsolver.solve("LLIW" + LE_hailfinder, hailfinder); 
            // GibsSolver.solve("LLIW" + LE_hailfinder, hailfinder); 
            // Little Evidence -------------------------------
            string MO_hailfinder = ",CombVerMo=Down, AreaMeso_ALS=Down, CurPropConv=Strong"; 
            // VEsolver.solve("SatContMoist" + MO_hailfinder, hailfinder); 
            // GibsSolver.solve("SatContMoist" + MO_hailfinder, hailfinder);
            // VEsolver.solve("LLIW" + MO_hailfinder, hailfinder); 
            // GibsSolver.solve("LLIW" + MO_hailfinder, hailfinder);  
           
            // Insurance Network [inputs passed]
            BaesNetwork insurance = new BaesNetwork("insurance.bif");
            // No Evidence
            // VEsolver.solve("MedCost", insurance); 
            // GibsSolver.solve("MedCost", insurance);
            // VEsolver.solve("ILiCost", insurance); 
            // GibsSolver.solve("IliCost", insurance);
            // VEsolver.solve("PropCost", insurance); 
            // GibsSolver.solve("PropCost", insurance);
            // Little Evidence ----------------------
            string LE_insurance = "|Age=Adolescent,GoodStudent=False,SeniorTrain=False,DrivQuality=Poor";
            // VEsolver.solve("MedCost" + LE_insurance, insurance); 
            // GibsSolver.solve("MedCost" + LE_insurance, insurance);
            // VEsolver.solve("ILiCost" + LE_insurance, insurance); 
            // GibsSolver.solve("IliCost" + LE_insurance, insurance);
            // VEsolver.solve("PropCost" + LE_insurance, insurance); 
            // GibsSolver.solve("PropCost" + LE_insurance, insurance); 
            // Moderate Evidence --------------------------
            string MO_insurance = LE_insurance + ",MakeModel=Luxury,CarValue=FiftyThou,DrivHist=Zero";
            // VEsolver.solve("MedCost" MO_insurance, insurance); 
            // GibsSolver.solve("MedCost" MO_insurance, insurance);
            // VEsolver.solve("ILiCost" MO_insurance, insurance); 
            // GibsSolver.solve("IliCost" MO_insurance, insurance);
            // VEsolver.solve("PropCost" MO_insurance, insurance); 
            // GibsSolver.solve("PropCost" MO_insurance, insurance);  

            // win95pts Network [inputes passed]
            BaesNetwork win95pts = new BaesNetwork("win95pts.bif");
            string[] report_win = new[] {"Problem1", "Problem2", "Problem3", "Problem4", "Problem5", "Problem6"};
            string[] evidence_win = new[] {
                "", "Problem1=No_Output", "Problem2=Too_Long", "Problem3=No", "Problem4=No", "Problem5=No", "Problem6=Yes"
            };
            foreach (string rep in report_win) {
                foreach (string evi in evidence_win) {
                    //VEsolver.solve(rep+evi, win95pts);
                    //GibsSolver.solve(rep+evi, win95pts);
                }
            }
        } 
    }
}
