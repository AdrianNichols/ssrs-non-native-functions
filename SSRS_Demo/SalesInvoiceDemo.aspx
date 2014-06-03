<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="SalesInvoiceDemo.aspx.cs" Inherits="Demonstrations.SalesInvoiceDemo" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
        function PostToNewWindow() {
            originalTarget = document.forms[0].target;
            document.forms[0].target = '_blank';
            window.setTimeout("document.forms[0].target=originalTarget;", 300);
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
       <asp:UpdatePanel ID="cultureSelectorDiv" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger controlid="reportSelector" eventname="SelectedIndexChanged"/>
        </Triggers>
            <ContentTemplate>
    <p>
        <asp:Label ID="lblReportSelector" Text="Select a Report:" runat="server"></asp:Label>
        <asp:DropDownList ID="reportSelector" OnSelectedIndexChanged="reportSelector_SelectedIndexChanged" AutoPostBack="true" runat="server" >
            <asp:ListItem Text="Stage 1: Layout" Value="SalesInvoice_1_Layout.rdlc"></asp:ListItem>
            <asp:ListItem Text="Stage 2: Raw Data" Value="SalesInvoice_2_RawData.rdlc"></asp:ListItem>
            <asp:ListItem Text="Stage 3: Header Data" Value="SalesInvoice_3_HeaderData.rdlc"></asp:ListItem>
            <asp:ListItem Text="Stage 4: Internationalisation" Value="SalesInvoice_4_Internationalisation.rdlc"></asp:ListItem>
            <asp:ListItem Text="Stage 5: Pin-to-bottom" Value="SalesInvoice_5_PinToBottom.rdlc"></asp:ListItem>
            <asp:ListItem Text="Stage 6: Data Driven Footer" Value="SalesInvoice_6_DataDrivenFooter.rdlc"></asp:ListItem>
            <asp:ListItem Text="Stage 7: Group X Paging" Value="SalesInvoice_7_GroupXPaging.rdlc"></asp:ListItem>
            <asp:ListItem Text="Stage 8: Group Of Y Paging" Value="SalesInvoice_8_GroupOfYPaging.rdlc"></asp:ListItem>
            <asp:ListItem Text="Stage 9: Finished Report" Value="SalesInvoice_9_FinishedReport.rdlc"></asp:ListItem>
            <asp:ListItem Text="Stage 10: Render in Another App Domain" Value="SalesInvoice_10_FinishedReport.rdlc"></asp:ListItem>
        </asp:DropDownList>
    </p>
    <div id="divcultureSelector" style="display:none" runat="server">
        <asp:Label ID="lblcultureSelector" Text="Select a Country:" runat="server"></asp:Label>
        <asp:DropDownList ID="cultureSelector" runat="server">
            <asp:ListItem Text="United Kingdom" Value="en-GB"></asp:ListItem>
            <asp:ListItem Text="United States" Value="en-US"></asp:ListItem>
            <asp:ListItem Text="Germany" Value="de-DE"></asp:ListItem>
        </asp:DropDownList>
    </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="cultureSelectorDiv">
            <ProgressTemplate>
                <span style="color:red">Configuring Controls... </span>
            </ProgressTemplate>
        </asp:UpdateProgress>
    <p>
        <asp:Button ID="executeReport" Text="Execute" UseSubmitBehavior="true" OnClientClick="return PostToNewWindow();" OnClick="executeReport_click" runat="server"/>
    </p>
    
</asp:Content>
