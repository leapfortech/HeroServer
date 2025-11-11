using System;

namespace HeroServer
{
    public class OfacSearchMatch
    {
#pragma warning disable IDE1006 // Naming Styles
        public String id { get; set; }
        public String type { get; set; }
        public String[] categories { get; set; }
        public String name { get; set; }
        public String nameFormatted { get; set; }
        public String entityLink { get; set; }
        public String source { get; set; }
        public String sourceId { get; set; }
        public String description { get; set; }
        public String remarks { get; set; }
        public String effectiveDate { get; set; }
        public String expirationDate { get; set; }
        public String lastUpdate { get; set; }
        public OfacAlias[] alias { get; set; }
        public OfacAddressResult[] addresses { get; set; }
        public OfacIdentificationResult[] identifications { get; set; }
        public String[] emailAddresses { get; set; }
        public String[] phoneNumbers { get; set; }
        public String[] websites { get; set; }
        public OfacCryptoWallet[] cryptoWallets { get; set; }
        public String[] sourceLinks { get; set; }
        public String[] programs { get; set; }
        public String[] additionalSanctions { get; set; }
        public OfacAdditionalInformation[] additionalInformation { get; set; }
        public OfacPersonDetails personDetails { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacSearchMatch()
        {
        }

        public OfacSearchMatch(String id, String type, String[] categories, String name, String nameFormatted, String entityLink, String source, String sourceId,
                               String description, String remarks, String effectiveDate, String expirationDate, String lastUpdate, OfacAlias[] alias, OfacAddressResult[] addresses,
                               OfacIdentificationResult[] identifications, String[] emailAddresses, String[] phoneNumbers, String[] websites, OfacCryptoWallet[] cryptoWallets,
                               String[] sourceLinks, String[] programs, String[] additionalSanctions, OfacAdditionalInformation[] additionalInformation, OfacPersonDetails personDetails)
        {
            this.id = id;
            this.type = type;
            this.categories = categories;
            this.name = name;
            this.nameFormatted = nameFormatted;
            this.entityLink = entityLink;
            this.source = source;
            this.sourceId = sourceId;
            this.description = description;
            this.remarks = remarks;
            this.effectiveDate = effectiveDate;
            this.expirationDate = expirationDate;
            this.lastUpdate = lastUpdate;
            this.alias = alias;
            this.addresses = addresses;
            this.identifications = identifications;
            this.emailAddresses = emailAddresses;
            this.phoneNumbers = phoneNumbers;
            this.websites = websites;
            this.cryptoWallets = cryptoWallets;
            this.sourceLinks = sourceLinks;
            this.programs = programs;
            this.additionalSanctions = additionalSanctions;
            this.additionalInformation = additionalInformation;
            this.personDetails = personDetails;
        }
    }
}
