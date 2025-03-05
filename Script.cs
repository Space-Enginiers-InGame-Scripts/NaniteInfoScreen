public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update100; // Update every 100 ticks
}

string stored_argument;

public void Main(string argument, UpdateType updateSource) {
    // if (updateSource == UpdateType.Update100) {
        
    // }
    if (stored_argument == null){
        stored_argument = argument;
    }
    if (stored_argument == null){
        return;
    }
    IMyTerminalBlock naniteFactory = GridTerminalSystem.GetBlockWithName("Nanite Control Facility " + stored_argument);
    // IMyTerminalBlock ultrasonicHummer = GridTerminalSystem.GetBlockWithName("NUHOL " + argument);
    List<IMyTerminalBlock> terminal_blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName("NANITE_LCD_" + stored_argument, terminal_blocks, block => block is IMyTextSurface);

    // Check if the LCD panel is found
    if (terminal_blocks == null) {
        throw new Exception($"NANITE_LCD_{stored_argument} not found!");
    }
    string panel_text = "-= Nanite Facility Status =-\n"; // naniteFactory.CustomName
    string scan_text = "--<Scan Info>--\n";
    string status_text = "--<Status>--\n";
    string parts_text = "---------------\nRequired Parts:\n";
    
    // Process Nanite Factory status
    if (naniteFactory != null) {
        string[] nfStatus = naniteFactory.CustomInfo.Split('\n');
        string cutom_name = naniteFactory.CustomName;
        panel_text += $"-= {cutom_name} =-\n";
        foreach (string line in nfStatus) {
            if (line.Contains("Scanning")) {
                scan_text += $"Scanning: {line.Split('-')[1].Trim()}\n";
                scan_text += $"Blocks: {nfStatus[Array.IndexOf(nfStatus, line) + 1].Split(' ')[0].Trim()}\n";
            } else if (line.Contains("Status:")) {
                status_text += $"State: {line.Split(':')[1].Trim()}\n";
            } else if (line.Contains("Active Nanites")) {
                status_text += $"Active: {line.Split(':')[1].Trim()}\n";
            } else if (line.Contains("Power Required")) {
                status_text += $"Power: {line.Split(':')[1].Trim()}\n";
            } else if (line.Contains("Needed parts")) {
                for (int j = Array.IndexOf(nfStatus, line) + 1; j < nfStatus.Length; j++) {
                    if (string.IsNullOrWhiteSpace(nfStatus[j]) || nfStatus[j].Contains("-----")) break; // Stop if we hit an empty line
                    parts_text += $"[{nfStatus[j].Trim()}]\n";
                }
            }
        }
        panel_text += scan_text;
        panel_text += status_text;
        panel_text += parts_text;
    } else {
        panel_text = "Nanite Control Facility not found or named incorrectly.\n";
        panel_text += $"DBG stored_argument: {stored_argument}\n";
        try {
            panel_text += $"DBG naniteFactory: {naniteFactory.CustomName}\n";
        } catch (Exception e){
            panel_text += $"DBG naniteFactory: {e.Message}\n";
        }
        
        foreach (IMyTerminalBlock tb in terminal_blocks) {
            panel_text += $"DBG lcds: {tb.CustomName}\n";
        }
        
    }

    foreach (IMyTextSurface textSurface in terminal_blocks) {
        textSurface.WriteText(panel_text);
    }
}
